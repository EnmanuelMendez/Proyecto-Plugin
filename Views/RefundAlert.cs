using Plugin_ICGFront.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Forms;

namespace Plugin_ICGFront.Views
{
    public partial class RefundAlert : BaseForm
    {
        private bool _appClosing;

        private int _remainingSeconds = 3;

        public RefundAlert(string eventName, RefundDetail refundDetail)
        {
            InitializeComponent();

            Color formColor;
            string warningText;

            switch (eventName)
            {
                default:
                    formColor = Color.Coral;
                    warningText =
                        $@"La factura {refundDetail.DocumentId} que origina la nota de crédito, fue creada en fecha {refundDetail.DocumentDate}, hace {refundDetail.NumberOfDays} días. Se procederá a descontar el ITBIS.";
                    break;
            }

            SetBackgroundColor(formColor);
            this.WarningText.Text = warningText;
        }

        [DllImport("USER32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void RefundAlert_Load(object sender, EventArgs e)
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
                var processName = "FrontRetail";
#else
                string processName = "notepad";
#endif
                var process = Process.GetProcessesByName(processName).FirstOrDefault();
                var mainWIndow = process.MainWindowHandle;
                var processElement = AutomationElement.FromHandle(mainWIndow);

                Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, processElement, TreeScope.Subtree,
                    (sender1, event1) =>
                    {
                        var element = sender1 as AutomationElement;
                        MessageBox.Show(element.Current.Name);
                    });
            }
            finally
            {
                CloseScreen();
            }
        }

        private void CloseScreen(DialogResult dialogResult = DialogResult.OK)
        {
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
            }

            base.OnFormClosing(e);
#endif
        }
    }
}