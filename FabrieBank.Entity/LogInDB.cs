using System;
using Microsoft.Data.SqlClient;
using System.Data.SQLite;
using FabrieBank.Common;
using FabrieBank.DAL;

namespace FabrieBank.Entity
{
    public class LogInDB
    {
        private DataAccessLayer dataAccessLayer;

        public LogInDB()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public DTOCustomer LogIn(long tckn, int sifre)
        {
            return dataAccessLayer.LogIn(tckn, sifre);
        }

        public bool UpdatePersonelInfo(int musteriId, long telNo, string email)
        {
            return dataAccessLayer.UpdatePersonelInfo(musteriId, telNo, email);
        }

        public bool IsCredentialsValid(long tckn, int sifre)
        {
            return dataAccessLayer.IsCredentialsValid(tckn, sifre);
        }
    }
}
