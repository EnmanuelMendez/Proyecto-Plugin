using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_ICGFront.Models
{
    public class PriceErrorDetail
    {
        public PriceErrorDetail(string itemCode, string description, string erroredPrice, string suggestedPrice)
        {
            this.ItemCode = itemCode;
            this.Description = description;
            this.ErroredPrice = erroredPrice;
            this.SuggestedPrice = suggestedPrice;
        }

        public string ItemCode { get; }
        public string Description { get; }
        public string ErroredPrice { get; }
        public string SuggestedPrice { get; }
    }
}
