using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Plugin_ICGFront.Views
{
    public partial class PromptDocument : BaseForm
    {
        public PromptDocument(string title = "")
        {
            InitializeComponent();

            if (title != "") Text = title;
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public string DocumentSeries { get; set; }
        public int DocumentNumber { get; set; }

        [DllImport("USER32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void PromptDocument_Load(object sender, EventArgs e)
        {
            DocSeries.Focus();
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            DocumentSeries = DocSeries.Text;
            DocumentNumber = (int)DocNumber.Value;
        }

        private void SeriesText_TextChanged(object sender, EventArgs e)
        {
            ContinueButton.Enabled = ValidSeriesAndNumber();
        }

        private void DocNumber_ValueChanged(object sender, EventArgs e)
        {
            ContinueButton.Enabled = ValidSeriesAndNumber();
        }

        private void DocNumber_KeyUp(object sender, KeyEventArgs e)
        {
            ContinueButton.Enabled = ValidSeriesAndNumber();
        }

        private void DocSeries_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar)) return;

            e.Handled = true;
        }

        private bool ValidSeriesAndNumber()
        {
            var series = DocSeries.Text;

            if (series.Length != 4)
                return false;

            var number = (int)DocNumber.Value;

            return number > 0;
        }

        private void DocNumber_Enter(object sender, EventArgs e)
        {
            var value = DocNumber.Value.ToString(CultureInfo.CurrentCulture);
            DocNumber.Select(0, value.Length);
        }

        private void DocNumber_Click(object sender, EventArgs e)
        {
            var value = DocNumber.Value.ToString(CultureInfo.CurrentCulture);

            if (value == "0") DocNumber.Select(0, value.Length);
        }

        private void DocNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter) ContinueButton.PerformClick();
        }
    }
}