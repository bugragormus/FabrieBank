namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOCurrencyRate
    {
        public string CurrencyCode { get; set; }
        public double BanknoteBuyingRate { get; set; }
        public double BanknoteSellingRate { get; set; }
        public double ForexBuyingRate { get; set; }
        public double ForexSellingRate { get; set; }
    }
}
