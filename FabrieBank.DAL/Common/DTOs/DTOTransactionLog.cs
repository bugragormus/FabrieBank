using FabrieBank.DAL.Common.Enums;

namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOTransactionLog
    {
        public int LogId { get; set; }
        public long SourceAccountNumber { get; set; }
        public long TargetAccountNumber { get; set; }
        public EnumTransactionType TransactionType { get; set; }
        public EnumTransactionStatus TransactionStatus { get; set; }
        public decimal TransferAmount { get; set; }
        public decimal TransferAmountSmall { get; set; }
        public decimal TransferAmountLarge { get; set; }
        public decimal CurrencyRate { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal SourceOldBalance { get; set; }
        public decimal SourceNewBalance { get; set; }
        public decimal TargetOldBalance { get; set; }
        public decimal TargetNewBalance { get; set; }
        public decimal TransactionFee { get; set; }
        public decimal KMV { get; set; }
        public decimal KMVSmall { get; set; }
        public decimal KMVLarge { get; set; }
    }
}