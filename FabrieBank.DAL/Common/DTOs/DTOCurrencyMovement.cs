using FabrieBank.DAL.Common.Enums;

namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOCurrencyMovement
    {
        public long SourceAccountNo { get; set; }
        public long TargetAccountNo { get; set; }
        public int SourceCurrencyType { get; set; }
        public int TargetCurrencyType { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public EnumTransactionType Type { get; set; }
    }
}