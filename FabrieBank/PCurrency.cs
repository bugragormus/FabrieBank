using System.Reflection;
using System.Text.RegularExpressions;
using FabrieBank.DAL;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;
using FabrieBank.DAL.Entity;
using FabrieBank.Services;

namespace FabrieBank
{
    public class PCurrency
    {
        private readonly SCurrency currency;

        public PCurrency()
        {
            currency = new SCurrency();
        }

        /// <summary>
        /// Calls 'DisplayCurrencyRatesTable' for Todays Rates
        /// </summary>
        public void TodaysRates()
        {
            try
            {
                var baseCurrency = EnumCurrencyTypes.CurrencyTypes.TRY;
                var currencyRates = currency.GetTodaysCurrencyRates(baseCurrency).Result;
                DisplayCurrencyRatesTable(baseCurrency, currencyRates);
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Calls 'DisplayCurrencyRatesTable' for Custome Dates
        /// </summary>
        public void CustomDateRates()
        {
            try
            {
                Console.WriteLine("Which date would you like to see the exchange rate information?");
                Console.WriteLine("Please enter the date in DD.MM.YYYY format.");
                string input = Console.ReadLine();

                if (IsValidDate(input, out int day, out int month, out int year))
                {
                    var baseCurrency = EnumCurrencyTypes.CurrencyTypes.TRY;

                    var currencyRates = currency.GetCustomDateCurrencyRates(baseCurrency, year, month, day).Result;

                    if (currencyRates.Count > 0)
                    {
                        DisplayCurrencyRatesTable(baseCurrency, currencyRates);
                    }
                    else
                    {
                        Console.WriteLine("Currency rates not found for the selected custom date.");
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Saves exchange rates into DTOCurrencyRates
        /// </summary>
        /// <param name="currencyRates"></param>
        /// <returns></returns>
        public List<DTOCurrencyRate> GetCurrencyRates(Dictionary<string, DTOCurrencyRate> currencyRates)
        {
            List<DTOCurrencyRate> dtoCurrencyRates = new List<DTOCurrencyRate>();

            foreach (var rate in currencyRates)
            {
                DTOCurrencyRate dtoCurrencyRate = new DTOCurrencyRate
                {
                    CurrencyCode = rate.Value.CurrencyCode,
                    ForexBuyingRate = rate.Value.ForexBuyingRate,
                    ForexSellingRate = rate.Value.ForexSellingRate,
                    BanknoteBuyingRate = rate.Value.BanknoteBuyingRate,
                    BanknoteSellingRate = rate.Value.BanknoteSellingRate
                };

                dtoCurrencyRates.Add(dtoCurrencyRate);
            }

            return dtoCurrencyRates;
        }

        /// <summary>
        /// Prints exchange rate table
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="currencyRates"></param>
        public void DisplayCurrencyRatesTable(EnumCurrencyTypes.CurrencyTypes baseCurrency, Dictionary<string, DTOCurrencyRate> currencyRates)
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
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Checks date format
        /// </summary>
        /// <param name="input"></param>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        static bool IsValidDate(string input, out int day, out int month, out int year)
        {
            string pattern = @"^\d{2}\.\d{2}\.\d{4}$"; // DD.MM.YYYY formatı için regex deseni
            if (!Regex.IsMatch(input, pattern))
            {
                Console.WriteLine("Geçersiz tarih formatı! Tarih (DD.MM.YYYY formatında olmalıdır.)");
                day = month = year = 0;
                return false;
            }

            string[] dateParts = input.Split('.');
            day = int.Parse(dateParts[0]);
            month = int.Parse(dateParts[1]);
            year = int.Parse(dateParts[2]);

            if (month < 1 || month > 12)
            {
                Console.WriteLine("Geçersiz ay! Ay değeri 1 ile 12 arasında olmalıdır.");
                return false;
            }
            else if (day < 1 || day > 31)
            {
                Console.WriteLine("Geçersiz gün! Gün değeri 1 ile 31 arasında olmalıdır.");
                return false;
            }
            else
            {
                if (month == 2)
                {
                    bool isLeapYear = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
                    if (day > 29 || (day == 29 && !isLeapYear))
                    {
                        Console.WriteLine("Geçersiz gün! Şubat için geçersiz gün değeri.");
                        return false;
                    }
                }
                else if (day > 30 && (month == 4 || month == 6 || month == 9 || month == 11))
                {
                    Console.WriteLine("Geçersiz gün! Bu ay için geçersiz gün değeri.");
                    return false;
                }
            }
            return true;
        }
    }
}