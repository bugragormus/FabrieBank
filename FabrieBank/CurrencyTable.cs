using System;
using System.Collections.Generic;
using System.Reflection;
using FabrieBank.Common.Enums;
using FabrieBank.Services;

namespace FabrieBank
{
    public class CurrencyTable
    {
        public void DisplayCurrencyRatesTable(EnumDovizCinsleri.DovizCinsleri baseCurrency)
        {
            try
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
