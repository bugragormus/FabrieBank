using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FabrieBank.Common.Enums;
using FabrieBank.Services;

namespace FabrieBank
{
    public class CurrencyTable
    {
        private readonly CurrencyService _currencyService;

        public CurrencyTable()
        {
            _currencyService = new CurrencyService();
        }

        public async Task DisplayCurrencyRatesTable(EnumDovizCinsleri.DovizCinsleri baseCurrency)
        {
            try
            {
                var currencyRates = await _currencyService.GetCurrencyRates(baseCurrency);

                Console.WriteLine("\n----------------------------------------------------------------------------------------");
                Console.WriteLine($"                Currency Rates Table for {baseCurrency} (Base Currency)");
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
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                FabrieBank.DAL.DataAccessLayer dataAccessLayer = new DAL.DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }
    }
}
