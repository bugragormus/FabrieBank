using FabrieBank.DAL.Common.Enums;
using FabrieBank.DAL;

namespace FabrieBank.Common.Entity
{
	public class TransactionFeeDB
	{
		private DataAccessLayer dataAccessLayer;

		public TransactionFeeDB()
		{
			dataAccessLayer = new DataAccessLayer();
		}

		public decimal GetTransactionFee(EnumTransactionFeeType transactionType)
		{
			return dataAccessLayer.GetTransactionFee(transactionType);
		}
    }
}