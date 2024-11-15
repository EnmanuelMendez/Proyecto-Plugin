using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_ICGFront.Models
{
    public class RaffleTicket
    {
        public RaffleTicket(string code, string invoiceSeries, int invoiceNumber, int id, string name, string info,
            DateTime startDate, DateTime endDate)
        {
            this.Code = code;
            this.InvoiceSeries = invoiceSeries;
            this.InvoiceNumber = invoiceNumber;
            PromotionId = id;
            PromotionName = name;
            PromotionInfo = info;
            PromotionStartDate = startDate;
            PromotionEndDate = endDate;
        }

        public string Code { get; }
        public string InvoiceSeries { get; }
        public int InvoiceNumber { get; }
        public int PromotionId { get; }
        public string PromotionName { get; }
        public string PromotionInfo { get; }
        public DateTime PromotionStartDate { get; }
        public DateTime PromotionEndDate { get; }
    }
}
