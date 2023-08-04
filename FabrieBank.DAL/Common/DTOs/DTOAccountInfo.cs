namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOAccountInfo
    {
        public long AccountNo { get; set; }
        public decimal Balance { get; set; }
        public int CustomerId { get; set; }
        public int CurrencyType { get; set; }
        public string? AccountName { get; set; }
    }
}