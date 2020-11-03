using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService.API.Models
{
    public class CreditRequest
    {
        public string AccountIdentifier { get; set; }
        public string SortCode { get; set; }
        public string PaymentReference { get; set; }

        public AmountClass TransferAmount { get; set; }
        public Creditor Creditor { get; set; }

    }
}
