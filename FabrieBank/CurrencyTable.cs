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
                MethodBase method = MethodBase.GetCurrentMethod();

                // Display a user-friendly error message
                Console.WriteLine("An error occurred while retrieving currency rates. Please try again later.");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
