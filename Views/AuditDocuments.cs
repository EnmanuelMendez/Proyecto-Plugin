using Plugin_ICGFront.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Plugin_ICGFront.Views
{
    public partial class AuditDocuments : BaseForm
    {
        private readonly List<ErroredDocumentDetail> _erroredDocuments;

        public AuditDocuments(List<ErroredDocumentDetail> erroredDocuments)
        {
            InitializeComponent();

            this._erroredDocuments = erroredDocuments;
        }

        [DllImport("USER32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void AuditDocuments_Load(object sender, EventArgs e)
        {
            var bindingSource = new BindingSource
            {
                DataSource = _erroredDocuments
            };

            dgv_items.DataSource = bindingSource;
            dgv_items.Enabled = true;
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}