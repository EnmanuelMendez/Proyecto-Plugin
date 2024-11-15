using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Plugin_ICGFront.Views
{
    public partial class CurrentRate : BaseForm
    {
        public CurrentRate(decimal usdRate)
        {
            InitializeComponent();

            txt_rate.Text = string.Format(@"{0} USD", usdRate);
        }

        [DllImport("USER32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void CurrentRate_Load(object sender, EventArgs e)
        {
        }

        private void Btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}