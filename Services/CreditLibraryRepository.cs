using CreditLibrary.API.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dapper;
using System.Data;

using CreditLibrary.API.Models;
using Microsoft.Data.SqlClient;
using PaymentService.API.Models;

namespace CreditLibrary.API.Services
{
    public class CreditLibraryRepository : ICreditLibraryRepository, IDisposable
    {

        private readonly string connstring;
        private IDbConnection Connection => new SqlConnection(connstring);
        public CreditLibraryRepository()
        {

            //  connstring = "Server=192.168.0.164;Database=maecbsdb;user=SA;Password=TCSuser1123;Trusted_Connection=True;";
            connstring = "Server=192.168.0.164;Database=maecbsdb;user=SA;Password=TCSuser1123;";
        }

        public bool CreditAmount(Credit req)
        {
            using (var c = Connection)
            {
                c.Open();
                var p = new DynamicParameters();
                p.Add("accountNumber", req.AccountIdentifier, DbType.String, ParameterDirection.Input);
                p.Add("amount", req.TransferAmount, DbType.Double, ParameterDirection.Input);

                string query = "update tb_mae_account set [amount]=[amount] + @amount where account_identifier= @accountNumber";
                var x = c.Query(query,p);
                                
                c.Close();
                return true;
            }           
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
