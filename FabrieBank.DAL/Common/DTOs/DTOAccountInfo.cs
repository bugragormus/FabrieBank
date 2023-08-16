namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOAccountInfo
    {
        public long AccountNo { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceIsSmall { get; set; }
        public decimal BalanceIsBig { get; set; }
        public int CustomerId { get; set; }
        public int CurrencyType { get; set; }
        public string? AccountName { get; set; }
    }
}