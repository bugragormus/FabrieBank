using System;
using FabrieBank.DAL.Common.Enums;

namespace FabrieBank.DAL.Common.DTOs
{
	public class DTOExchange
	{
		public decimal ExchangeRate { get; set; }
		public EnumCurrencyTypes.CurrencyTypes CurrencyType { get; set; }
		public long SourceAccountNo { get; set; }
		public decimal SourceAccountBalance { get; set; }
		public long TargetAccountNo { get; set; }
		public decimal TargetAccountBalance { get; set; }
		public decimal Amount { get; set; }
    }
}

