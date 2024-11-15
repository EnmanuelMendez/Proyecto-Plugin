using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_ICGFront.Models
{
    public class RefundDetail
    {
        public RefundDetail(string documentId, string documentDate, int numberOfDays)
        {
            this.DocumentId = documentId;
            this.DocumentDate = documentDate;
            this.NumberOfDays = numberOfDays;
            GreaterThan30Days = numberOfDays > 30;
        }

        public string DocumentId { get; }
        public string DocumentDate { get; }
        public int NumberOfDays { get; }
        public bool GreaterThan30Days { get; }
    }
}
