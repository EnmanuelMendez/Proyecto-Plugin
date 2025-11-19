using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_ICGFront.Models
{
    public class CreditNoteDetail
    {
        public CreditNoteDetail(string documentId, string tasaFacturaOriginal, string tasaNotaCredito, bool tasaDiferente)
        {
            this.DocumentId = documentId;
            this.TasaFacturaOriginal = tasaFacturaOriginal;
            this.TasaNotaCredito = tasaNotaCredito;
            this.TasaDiferente = tasaDiferente;
        }

        public string DocumentId { get; }
        public string TasaFacturaOriginal { get; }
        public string TasaNotaCredito { get; } 
        public bool TasaDiferente { get; }
    }
}

