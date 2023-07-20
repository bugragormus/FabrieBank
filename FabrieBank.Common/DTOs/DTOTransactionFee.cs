using FabrieBank.Common.Enums;

namespace FabrieBank.Common.DTOs
{
    public class DTOTransactionFee
    {
        public long FeeId { get; set; }
        public EnumTransactionFeeType FeeType { get; set; }
        public decimal Amount { get; set; }
    }
}
