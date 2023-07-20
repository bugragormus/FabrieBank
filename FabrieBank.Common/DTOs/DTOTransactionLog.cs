using FabrieBank.Common.Enums;
using System;

namespace FabrieBank.Common.DTOs
{
    public class DTOTransactionLog
    {
        public long LogId { get; set; }
        public long AccountNumber { get; set; }
        public long TargetAccountNumber { get; set; }
        public EnumTransactionType TransactionType { get; set; }
        public EnumTransactionStatus TransactionStatus { get; set; }
        public long Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public long OldBalance { get; set; }
        public long NewBalance { get; set; }
    }
}
