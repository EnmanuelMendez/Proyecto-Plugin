using System;
using System.Windows.Forms;

namespace Plugin_ICGFront.Views
{
    public partial class ChangeRateCreditMemo : Form
    {
        private readonly ParsedDocument _parsedDocument;
        private decimal tasa_sistema = -1;
        private decimal? tasa_factura = -1;

        public ChangeRateCreditMemo(ParsedDocument parsedDocument)
        {
            this._parsedDocument = parsedDocument;
            InitializeComponent();

            this.MaximizeBox = false;
            this.MinimizeBox = false;

            //obtener tasa del sistema y de la factura
            ObtenerTasas();

            btn_cambiar_tasa.Click += btn_cambiar_tasa_Click;
            btn_cancelar.Click += btn_cancelar_Click;
        }

        private void ObtenerTasas()
        {
            this.tasa_sistema = this._parsedDocument.GetCurrencyRate();
            this.tasa_factura = this._parsedDocument.GetInvoiceRate();

            if (this.tasa_factura == null)
            {
                MessageBox.Show(
                    "No se encontró una factura con esa serie y número.",
                    "Factura no encontrada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            this.lb_pregunta.Text = $"¿DESEA CAMBIAR LA TASA DEL SISTEMA A {tasa_factura.Value:N4}?";

            this.txt_tasa_sistema.Text = tasa_sistema.ToString();
            this.txt_tasa_factura.Text = tasa_factura.ToString();
        }

        private void btn_cambiar_tasa_Click(object sender, EventArgs e)
        {
            string mensaje =
                $"Tasa actual del sistema: {this.tasa_sistema:N4}\n" +
                $"Tasa de la factura: {this.tasa_factura.Value:N4}\n\n" +
                $"¿Desea cambiar la tasa actual a {this.tasa_factura.Value:N4}?";

            DialogResult result = MessageBox.Show(
                    mensaje,
                    "Confirmar cambio de tasa",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                );

            if (result != DialogResult.OK)
            {
                return;
            }

            try
            {
                bool actualizado = _parsedDocument.ChangeRateForCreditMemo(this.tasa_factura.Value);

                if (actualizado)
                {
                    MessageBox.Show(
                        "La tasa fue actualizada correctamente en MONEDAS y COTIZACIONES.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    Close();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo actualizar la tasa.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al cambiar la tasa: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btn_cancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txt_serie_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
