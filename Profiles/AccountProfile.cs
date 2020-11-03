using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditLibrary.API.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
         
        }

        private DateTime MapTransactionDate(string transaction_Date)
        {
            return Convert.ToDateTime(transaction_Date);
        }
    }
}
