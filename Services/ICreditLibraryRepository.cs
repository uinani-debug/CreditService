using CreditLibrary.API.Entities;
using PaymentService.API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace CreditLibrary.API.Services
{
    public interface ICreditLibraryRepository
    {       
       bool CreditAmount(Credit accountNumber);
    //    bool DebitAmount(PaymentRequest accountNumber);

    }
}
