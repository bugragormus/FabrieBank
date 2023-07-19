using System;
using System.Reflection;
using FabrieBank.Common;
using FabrieBank.DAL;
using Microsoft.Data.SqlClient;

namespace FabrieBank.Entity
{
    public class ATM
    {
        private DataAccessLayer dataAccessLayer;

        public ATM()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public void ParaYatirma(long hesapNo, long bakiye)
        {
            dataAccessLayer.Deposit(hesapNo, bakiye);
        }

        public void ParaCekme(long hesapNo, long bakiye)
        {
            dataAccessLayer.Withdraw(hesapNo, bakiye);
        }
    }
}