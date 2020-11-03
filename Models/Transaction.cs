using System;

namespace CreditLibrary.API.Models
{
    public class Transaction
    {
        public string transactionName { get; set; }
        public DateTime transactionDate { get; set; }
        public decimal transactionAmount { get; set; }
        public string transactionType { get; set; }
        public decimal currentBalance { get; set; }

    }

}
