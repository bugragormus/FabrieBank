using System;
using System.Collections.Generic;
using FabrieBank.Common.Enums;
using FabrieBank.Services;

namespace FabrieBank
{
    public class CurrencyTable
    {
        public void DisplayCurrencyRatesTable(EnumDovizCinsleri.DovizCinsleri baseCurrency)
        {
            var currencyService = new CurrencyService();
            var currencyRates = currencyService.GetCurrencyRates(baseCurrency).Result;

            Console.WriteLine($"Currency Rates Table for {baseCurrency} (Base Currency)");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Currency  |  Exchange Rate (TRY)");
            Console.WriteLine("--------------------------------");

            foreach (var rate in currencyRates)
            {
                Console.WriteLine($"{rate.Key,-8}  |  {rate.Value,15:N2}");
            }
        }
    }
}
