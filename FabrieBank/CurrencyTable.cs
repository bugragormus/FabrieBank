using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;
using FabrieBank.DAL.Entity;

namespace FabrieBank
{
    public class CurrencyTable
    {
        public void DisplayCurrencyRatesTable(EnumDovizCinsleri.DovizCinsleri baseCurrency, Dictionary<string, DTOCurrencyRate> currencyRates)
        {
            try
            {
                Console.WriteLine("\n----------------------------------------------------------------------------------------");
                Console.WriteLine($"                    Currency Rates Table for {baseCurrency} (Base Currency)");
                Console.WriteLine("----------------------------------------------------------------------------------------");
                Console.WriteLine("Currency  |  Forex Buying    |  Forex Selling   |  Banknote Buying   |  Banknote Selling");
                Console.WriteLine("----------------------------------------------------------------------------------------");

                foreach (var rate in currencyRates)
                {
                    Console.WriteLine($"{rate.Key,-8}  |  {rate.Value.ForexBuyingRate,14:N2}  |  {rate.Value.ForexSellingRate,14:N2}  |  {rate.Value.BanknoteBuyingRate,16:N2}  |  {rate.Value.BanknoteSellingRate,16:N2}");
                }
            }
            catch (Exception ex)
            {
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }
        }
    }
}