using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace Plugin_ICGFront.Views
{
    public partial class PasteSerialNumbers : BaseForm
    {
        public PasteSerialNumbers()
        {
            InitializeComponent();
        }

        [DllImport("USER32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void PasteSerialNumbers_Load(object sender, EventArgs e)
        {
            btnPasteSN.Enabled = false;
            txtSerialNumbers.Focus();
        }

        private void TxtSerialNumbers_TextChanged(object sender, EventArgs e)
        {
            btnPasteSN.Enabled = txtSerialNumbers.Text.Length > 0;
        }

        private void BtnPasteSN_Click(object sender, EventArgs e)
        {
            if (txtSerialNumbers.Text.Length == 0)
                return;

            try
            {
#if !DEBUG
                var processName = "FrontRetail";
#else
                string processName = "notepad";
#endif
                var process = Process.GetProcessesByName(processName).FirstOrDefault();
                var inputSimulator = new InputSimulator();

                if (process != null)
                {
                    var handle = process.MainWindowHandle;
                    Thread.Sleep(500);
                    SetForegroundWindow(handle);

                    foreach (var serialNumber in txtSerialNumbers.Lines)
                    {
                        if (serialNumber.Length == 0)
                            continue;
                        inputSimulator.Keyboard.TextEntry(serialNumber);
                        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.DOWN);
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                Close();
            }
        }
    }
}