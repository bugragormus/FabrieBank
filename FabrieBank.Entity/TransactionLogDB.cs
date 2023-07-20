using System;
using System.Data;
using Microsoft.Data.SqlClient;
using FabrieBank.Common.DTOs;
using FabrieBank.Common.Enums;
using FabrieBank.DAL;

namespace FabrieBank.Entity
{
    public class TransactionLogDB
    {
        private DataAccessLayer dataAccessLayer;

        public TransactionLogDB()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public void InsertTransactionLog(DTOTransactionLog transactionLog)
        {
            dataAccessLayer.LogTransaction(transactionLog);
        }
    }
}
