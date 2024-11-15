using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_ICGFront.Models
{
    public class ErroredDocumentDetail
    {
        public ErroredDocumentDetail(string series, int number, string cardCode, string cardName, string docDate,
            string docCur, double docTotal)
        {
            this.Series = series;
            this.Number = number;
            this.CardCode = cardCode;
            this.CardName = cardName;
            this.DocDate = docDate;
            this.DocCur = docCur;
            this.DocTotal = docTotal;
        }

        public string Series { get; }
        public int Number { get; }
        public string CardCode { get; }
        public string CardName { get; }
        public string DocDate { get; }
        public string DocCur { get; }
        public double DocTotal { get; }
    }
}
