using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Forms;

// Hola
namespace Plugin_ICGFront.Views
{
    public partial class AuditInvoice : BaseForm
    {
        private bool _appClosing;

        private int _remainingSeconds = 5;

        public AuditInvoice(string eventName, bool hasPriceErrors, bool hasDocumentCurrencyMismatch)
        {
            InitializeComponent();

            var formColor = Color.Transparent;
            var warningText = "";

            switch (eventName)
            {
                case "pending-sale":
                    if (hasPriceErrors)
                    {
                        formColor = Color.LightPink;
                        warningText =
                            "El documento contiene errores en los precios. Por favor, recupere la venta y revise que esten correctos.";
                    }

                    break;
                case "audit-quote":
                    if (hasDocumentCurrencyMismatch)
                    {
                        formColor = Color.LightGreen;
                        warningText =
                            "Las cotizaciones deben guardarse en dólares. Recupere el pedido con facturar todo, use moneda dólares y vuelva a cotizaciones para guardarla.";
                    }

                    break;
                default:
                    if (hasPriceErrors)
                    {
                        formColor = Color.PeachPuff;
                        warningText =
                            "El documento contiene errores en los precios. Por favor, vuelva a la pantalla anterior y revise que esten correctos.";
                    }
                    else if (hasDocumentCurrencyMismatch)
                    {
                        formColor = Color.LightSkyBlue;
                        warningText =
                            "Este documento no es valido para esta caja. Por favor, verifique que en esta caja se pueda crear documentos con la moneda especificada.";
                    }

                    break;
            }

            SetBackgroundColor(formColor);
            this.WarningText.Text = warningText;
        }

        [DllImport("USER32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void AuditInvoice_Load(object sender, EventArgs e)
        {
            ResizeElements();
            SetRandomDismissButtonPosition();
            CountdownTimer.Start();
        }

        private void ResizeElements()
        {
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;

            AlertPictureBox.Height = Convert.ToInt32(screenHeight / 3);
            AlertPictureBox.SetBounds(0, AlertPictureBox.Bounds.Y * 3, screenWidth, AlertPictureBox.Height);
            WarningText.Height = Convert.ToInt32(screenHeight / 2.5);
            WarningText.SetBounds(0, AlertPictureBox.Bounds.Y + AlertPictureBox.Height, screenWidth,
                WarningText.Height);
            WaitingText.SetBounds(0, 0, screenWidth, WaitingText.Height);
            DismissButtonArea.Height = Convert.ToInt32(screenHeight / 4);
            DismissButtonArea.SetBounds(0, screenHeight - DismissButtonArea.Height, screenWidth,
                DismissButtonArea.Height);
        }

        private void SetRandomDismissButtonPosition()
        {
            var maxX = DismissButtonArea.Width;
            var maxY = DismissButtonArea.Height;
            var buttonWidth = DismissButton.Width;
            var buttonHeight = DismissButton.Height;

            var randomNumber = new Random();

            var buttonX = randomNumber.Next(1, maxX - buttonWidth);
            var buttonY = randomNumber.Next(1, maxY - buttonHeight);
            DismissButton.SetBounds(buttonX, buttonY, buttonWidth, buttonHeight);
        }

        private void SetBackgroundColor(Color color)
        {
            BackColor = AlertPictureBox.BackColor =
                WarningText.BackColor = WaitingText.BackColor = DismissButtonArea.BackColor = color;
        }

        private void DismissButton_Click(object sender, EventArgs e)
        {
            try
            {
#if !DEBUG
                const string processName = "FrontRetail";
#else
                string processName = "notepad";
#endif
                var process = Process.GetProcessesByName(processName).FirstOrDefault();
                if (process == null) return;
                var mainWIndow = process.MainWindowHandle;
                var processElement = AutomationElement.FromHandle(mainWIndow);

                Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, processElement, TreeScope.Subtree,
                    (sender1, event1) =>
                    {
                        var element = sender1 as AutomationElement;
                        if (element != null) MessageBox.Show(element.Current.Name);
                    });
            }
            finally
            {
                CloseScreen();
            }
        }

        private void CloseScreen(DialogResult dialogResult = DialogResult.OK)
        {
            //this.StartSendingEsc();
            _appClosing = true;
            DialogResult = dialogResult;
            Close();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            WaitingText.Text = string.Format(@"Espere {0} segundos...", _remainingSeconds);

            if (_remainingSeconds == 0)
            {
                DismissButton.Visible = true;
                DismissButton.Enabled = true;
                WaitingText.Visible = false;
                CountdownTimer.Stop();
            }

            _remainingSeconds--;
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

        //private void StartSendingEsc()
        //{
        //    string filename = Path.Combine(AppDomain.CurrentDomain.FriendlyName);
        //    Process.Start(filename, "send-esc");
        //    this.AppClosing = true;
        //    this.DialogResult = DialogResult.OK;
        //}
    }
}