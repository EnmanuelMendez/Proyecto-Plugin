using System;
using System.Windows.Forms;

namespace Plugin_ICGFront.Views
{
    public partial class DiscountCalculator : BaseForm
    {
        public DiscountCalculator()
        {
            InitializeComponent();
        }

        private void UpdateDiscount()
        {
            var defaultPrice = nupDefaultPrice.Value;
            var newPrice = nupNewPrice.Value;

            if (defaultPrice < newPrice)
            {
                nupDiscount.Value = 0;
                return;
            }

            var discount = (defaultPrice - newPrice) / defaultPrice * 100;
            nupDiscount.Value = discount;
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (nupDiscount.Value == 0)
                return;

            Clipboard.SetText(nupDiscount.Value.ToString("0.########"));
        }

        private void NupDefaultPrice_ValueChanged(object sender, EventArgs e)
        {
            if (nupDefaultPrice.Value == 0)
                return;

            nupNewPrice.Maximum = nupDefaultPrice.Value;

            UpdateDiscount();
        }

        private void NupNewPrice_ValueChanged(object sender, EventArgs e)
        {
            if (nupNewPrice.Value == 0)
                return;

            UpdateDiscount();
        }

         
    }
}