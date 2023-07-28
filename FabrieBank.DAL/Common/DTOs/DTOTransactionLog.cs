using FabrieBank.Common.Enums;

namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOTransactionLog
    {
        public long LogId { get; set; }
        public long AccountNumber { get; set; }
        public long TargetAccountNumber { get; set; }
        public EnumTransactionType TransactionType { get; set; }
        public EnumTransactionStatus TransactionStatus { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal OldBalance { get; set; }
        public decimal NewBalance { get; set; }
        public decimal TransactionFee { get; set; }
    }
}
