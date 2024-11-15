using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;
using Plugin_ICGFront.Models;

namespace Plugin_ICGFront.Views
{
    public partial class CustomClient : BaseForm
    {
        private readonly ParsedDocument _currentParsedDocument;
        private bool _appClosing;
        private ApiResponseDGII _contributorInfo;
        private bool _manualMode;

        public CustomClient(ParsedDocument parsedDocument)
        {
            InitializeComponent();
            _currentParsedDocument = parsedDocument;

            paymentSchemeCb.DisplayMember = "Text";
            paymentSchemeCb.ValueMember = "Value";
        }

        [DllImport("USER32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void CustomClient_Load(object sender, EventArgs e)
        {
            rncTxt.Focus();

            var isRefund = _currentParsedDocument.DocumentFutureSeries.EndsWith("H");

            if (!isRefund) return;

            _contributorInfo = new ApiResponseDGII
            {
                Data = _currentParsedDocument.GetContributorInformationFromInvoice(),
                Valid = true
            };

            if (_contributorInfo.Data == null)
            {
                Utilities.ShowAlert("No se pudo obtener los datos para la devolucion.");
                CloseScreen();

                return;
            }

            paymentSchemeCb.DataSource = null;

            cdRncTxt.Text = _contributorInfo.Data.Rnc;
            cdRncTxt.ReadOnly = true;

            cdNameTxt.Text = _contributorInfo.Data.Name;
            cdNameTxt.ReadOnly = true;

            cdStatusTxt.Text = _contributorInfo.Data.Status == "2" ? "ACTIVO" : "-";
            cdStatusTxt.ReadOnly = true;

            infoPane.Visible = true;

            var cbItems = new List<object>
            {
                new { Text = "SELECCIONE UNA OPCION...", Value = "00" }
            };

            switch (_contributorInfo.Data.PaymentScheme)
            {
                case "2":
                    cbItems.Add(new { Text = "CONSUMIDOR FINAL", Value = "02" });
                    break;
                case "1":
                    cbItems.Add(new { Text = "CREDITO FISCAL", Value = "01" });
                    break;
            }

            paymentSchemeCb.DataSource = cbItems;

            ResetForm();

            searchBtn.Enabled = false;
            rncTxt.Enabled = false;
            continueBtn.Enabled = false;
        }

        private void CloseScreen(DialogResult dialogResult = DialogResult.OK)
        {
            _appClosing = true;
            DialogResult = dialogResult;
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
#if !DEBUG
            switch (e.CloseReason)
            {
                case CloseReason.UserClosing:
                    e.Cancel = !_appClosing;
                    break;
                case CloseReason.None:
                    break;
                case CloseReason.WindowsShutDown:
                    break;
                case CloseReason.MdiFormClosing:
                    break;
                case CloseReason.TaskManagerClosing:
                    break;
                case CloseReason.FormOwnerClosing:
                    break;
                case CloseReason.ApplicationExitCall:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.OnFormClosing(e);
#endif
        }

        private async void FetchDataFromDGII(string rnc)
        {
            searchBtn.Enabled = false;
            rncTxt.Enabled = false;
            _contributorInfo = null;

            var cbItems = new List<object>
            {
                new { Text = "SELECCIONE UNA OPCION...", Value = "00" },
                new { Text = "CONSUMIDOR FINAL", Value = "02" }
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.digital.gob.do/v1/contributors/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(10);

                try
                {
#if DEBUG
                    throw new Exception();
#endif
                    var response = await client.GetAsync($@"{rnc}/info/basic");

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        _contributorInfo = JsonConvert.DeserializeObject<ApiResponseDGII>(jsonString);
                    }
                    else
                    {
                        throw new Exception("El RNC digitado no existe o el servicio de la DGII no esta disponible.");
                    }
                }
                catch (Exception)
                {
                    var localResult = _currentParsedDocument.GetContributorInfo(rnc);
                    _contributorInfo = new ApiResponseDGII
                    {
                        Valid = localResult != null,
                        Data = localResult
                    };
                }
            }

            if (!_contributorInfo.Valid)
            {
                var decision =
                    Utilities.ShowAlert(
                        "El RNC digitado no es existe o no es valido. ¿Desea digitar los datos manualmente?",
                        "Atencion", MessageBoxIcon.Exclamation, MessageBoxButtons.YesNo);

                if (decision == DialogResult.No)
                {
                    ResetForm(false);
                    return;
                }

                EnableInputFields();
                paymentSchemeCb.DataSource = cbItems;
                return;
            }

            paymentSchemeCb.DataSource = null;

            cdRncTxt.Text = _contributorInfo.Data.Rnc;
            cdNameTxt.Text = _contributorInfo.Data.Name;
            cdStatusTxt.Text = _contributorInfo.Data.Status == "2" ? "ACTIVO" : "-";
            infoPane.Visible = _contributorInfo.Valid;

            if (_contributorInfo.Data.PaymentScheme == "2") cbItems.Add(new { Text = "CREDITO FISCAL", Value = "01" });

            paymentSchemeCb.DisplayMember = "Text";
            paymentSchemeCb.ValueMember = "Value";

            paymentSchemeCb.DataSource = cbItems;

            ResetForm();
        }

        private void ResetForm(bool canContinue = true)
        {
            _manualMode = false;
            rncTxt.Text = "";
            searchBtn.Enabled = true;
            rncTxt.Enabled = true;
            infoPane.Visible = canContinue;
            continueBtn.Enabled = canContinue;
        }

        private void EnableInputFields()
        {
            _manualMode = true;
            cdRncTxt.Enabled = true;
            cdRncTxt.ReadOnly = false;
            cdRncTxt.Text = rncTxt.Text;
            cdNameTxt.Enabled = true;
            cdNameTxt.ReadOnly = false;
            cdNameTxt.Text = "";
            cdStatusTxt.Text = @"ACTIVO";
            infoPane.Visible = true;
        }

        private void RNC_Txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            e.Handled = false;
        }

        private void RNC_Txt_TextChanged(object sender, EventArgs e)
        {
            searchBtn.Enabled = rncTxt.Text.Length >= 9;
        }

        private void Search_Btn_Click(object sender, EventArgs e)
        {
            var rnc = rncTxt.Text;

            FetchDataFromDGII(rnc);
        }

        private void Cancel_Btn_Click(object sender, EventArgs e)
        {
            CloseScreen(DialogResult.Cancel);
        }

        private void PaymentScheme_Cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                continueBtn.Enabled = (string)paymentSchemeCb.SelectedValue != "00";
            }
            catch
            {
                continueBtn.Enabled = false;
            }
        }

        private void Continue_Btn_Click(object sender, EventArgs e)
        {
            if (_manualMode)
            {
                var data = new ContributorInfo
                {
                    Rnc = cdRncTxt.Text,
                    Name = cdNameTxt.Text,
                    CommercialName = "",
                    PaymentScheme = "2",
                    Status = "2"
                };

                _contributorInfo = new ApiResponseDGII
                {
                    Valid = true,
                    Data = data
                };
            }

            if (_contributorInfo.Data == null || !_contributorInfo.Valid)
            {
                Utilities.ShowAlert("Por favor, indique un contribuyente valido.");
                return;
            }

            var selectedPaymentScheme = (string)paymentSchemeCb.SelectedValue;

            if (selectedPaymentScheme == "00")
            {
                Utilities.ShowAlert("Por favor, seleccione un tipo de comprobante valido.");
                return;
            }

            var updatePayload = new ContributorUpdatePayload(
                _contributorInfo.Data.Rnc,
                _contributorInfo.Data.Name,
                selectedPaymentScheme.Replace("0", string.Empty)
            );

            var success = _currentParsedDocument.UpdateContributorInformation(updatePayload);

            if (!success)
            {
                Utilities.ShowAlert("No se pudo actualizar los datos para la venta.");
                return;
            }

            CloseScreen();
        }

        private void RNC_Txt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            var rnc = rncTxt.Text;

            FetchDataFromDGII(rnc);
        }
    }

    public class ApiResponseDGII
    {
        public bool Valid { get; set; }
        public ContributorInfo Data { get; set; }
    }
}