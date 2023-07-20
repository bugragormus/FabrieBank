using System;
using FabrieBank.Common.Enums;
using FabrieBank.DAL;

namespace FabrieBank.Entity
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