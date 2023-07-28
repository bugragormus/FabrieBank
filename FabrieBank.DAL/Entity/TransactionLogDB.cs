using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL;

namespace FabrieBank.DAL.Entity
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
