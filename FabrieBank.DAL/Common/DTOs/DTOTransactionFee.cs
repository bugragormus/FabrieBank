using FabrieBank.DAL.Common.Enums;

namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOTransactionFee
    {
        public EnumTransactionFeeType FeeType { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountIsSmall { get; set; }
        public decimal AmountIsBig { get; set; }
    }
}