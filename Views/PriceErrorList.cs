using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Plugin_ICGFront.Views
{
    public partial class PriceErrorList : BaseForm
    {
        private readonly ParsedDocument _parsedDocument;

        public PriceErrorList(ParsedDocument parsedDocument)
        {
            InitializeComponent();

            this._parsedDocument = parsedDocument;
        }

        [DllImport("USER32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void PriceErrorList_Load(object sender, EventArgs e)
        {
            var errorList = _parsedDocument.GetDetailedPriceErrors();
            var bindingSource = new BindingSource
            {
                DataSource = errorList
            };

            dgvItems.DataSource = bindingSource;
            dgvItems.Enabled = true;
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Dgv_items_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (!(senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn) || e.RowIndex < 0) return;

            var suggestedPriceColumn = senderGrid.Columns["SuggestedPrice"];

            var cellValue = senderGrid.Rows[e.RowIndex].Cells[suggestedPriceColumn.Index].Value.ToString();
            Clipboard.SetText(cellValue);
        }
    }
}