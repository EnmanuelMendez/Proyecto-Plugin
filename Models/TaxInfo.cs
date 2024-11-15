using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_ICGFront.Models
{
    public class TaxInfo
    {
        public TaxInfo(bool active, bool notExpired, int remainingReceipts, string nextReceipt, string receiptType,
            bool uniqueReceipt, string customerType, decimal netTotal, string documentSeries, string receiptNumber)
        {
            this.Active = active;
            this.NotExpired = notExpired;
            this.RemainingReceipts = remainingReceipts;
            this.NextReceipt = nextReceipt;
            this.ReceiptType = receiptType;
            this.UniqueReceipt = uniqueReceipt;
            this.CustomerType = customerType;
            this.NetTotal = netTotal;
            this.DocumentSeries = documentSeries;
            this.ReceiptNumber = receiptNumber;
        }

        public bool Active { get; }
        public bool NotExpired { get; }
        public int RemainingReceipts { get; }
        public string NextReceipt { get; }
        public string ReceiptType { get; }
        public bool UniqueReceipt { get; }
        public string CustomerType { get; }
        public decimal NetTotal { get; }
        public string DocumentSeries { get; }
        public string ReceiptNumber { get; }
    }
}
