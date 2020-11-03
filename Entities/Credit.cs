using PaymentService.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CreditLibrary.API.Entities
{
    public class Credit
    {
        public string AccountIdentifier { get; set; }
        public string SortCode { get; set; }
        public string PaymentReference { get; set; }

        public double TransferAmount { get; set; }

    }
}
