using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_ICGFront.Models
{
    public class ContributorUpdatePayload
    {
        public ContributorUpdatePayload(string rnc, string name, string paymentScheme)
        {
            this.Rnc = rnc;
            this.Name = name;
            this.PaymentScheme = paymentScheme;
        }

        public string Rnc { get; }
        public string Name { get; }
        public string PaymentScheme { get; }
    }
}
