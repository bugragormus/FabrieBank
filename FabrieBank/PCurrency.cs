using System.Text.RegularExpressions;
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
            currencyTable = new CurrencyTable();
        }

        public void TodaysRates()
        {
            try
            {
                var baseCurrency = EnumCurrencyTypes.CurrencyTypes.TRY;//TRY
                var currencyRates = currency.GetTodaysCurrencyRates(baseCurrency).Result;
                currencyTable.DisplayCurrencyRatesTable(baseCurrency, currencyRates);
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

        public void CustomDateRates()
        {
            try
            {
                Console.WriteLine("Which date would you like to see the exchange rate information?");
                Console.WriteLine("Please enter the date in DD/MM/YYYY format.");
                string input = Console.ReadLine();

                if (IsValidDate(input, out int day, out int month, out int year))
                {
                    var baseCurrency = EnumCurrencyTypes.CurrencyTypes.TRY;//TRY

                    // Fetch the currency rates for the custom date
                    var currencyRates = currency.GetCustomDateCurrencyRates(baseCurrency, year, month, day).Result;

                    // Check if currency rates are available for the custom date
                    if (currencyRates.Count > 0)
                    {
                        currencyTable.DisplayCurrencyRatesTable(baseCurrency, currencyRates);
                    }
                    else
                    {
                        Console.WriteLine("Currency rates not found for the selected custom date.");
                    }
                }
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

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
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

        static bool IsValidDate(string input, out int day, out int month, out int year)
        {
            string pattern = @"^\d{2}/\d{2}/\d{4}$"; // DD/AA/YYYY formatı için regex deseni
            if (!Regex.IsMatch(input, pattern))
            {
                Console.WriteLine("Invalid date format! Date (must be in DD/MM/YYYY format.)");
                day = month = year = 0;
                return false;
            }

            string[] dateParts = input.Split('/');
            day = int.Parse(dateParts[0]);
            month = int.Parse(dateParts[1]);
            year = int.Parse(dateParts[2]);

            // Tarih geçerlilik kontrolü
            if (month < 1 || month > 12)
            {
                Console.WriteLine("Invalid month! The month value must be between 1 and 12.");
                return false;
            }
            else if (day < 1 || day > 31)
            {
                Console.WriteLine("Invalid day! The day value must be between 1 and 31.");
                return false;
            }
            else
            {
                // Şubat ayı için özel kontrol
                if (month == 2)
                {
                    // Şubat ayı 28 veya 29 gün çekebilir (artık yıl kontrolü yapmalıyız)
                    bool isLeapYear = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
                    if (day > 29 || (day == 29 && !isLeapYear))
                    {
                        Console.WriteLine("Invalid day! Invalid day value for February.");
                        return false;
                    }
                }
                // Diğer aylar için gün sayısı kontrolü
                else if (day > 30 && (month == 4 || month == 6 || month == 9 || month == 11))
                {
                    Console.WriteLine("Invalid day! Invalid day value for this month.");
                    return false;
                }
            }
            return true;
        }
    }
}