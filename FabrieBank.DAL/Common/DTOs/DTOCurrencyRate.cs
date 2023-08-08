namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOCurrencyRate
    {
        public string? CurrencyCode { get; set; }
        public decimal BanknoteBuyingRate { get; set; }
        public decimal BanknoteSellingRate { get; set; }
        public double ForexBuyingRate { get; set; }
        public decimal ForexSellingRate { get; set; }
    }
}