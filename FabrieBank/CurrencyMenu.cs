using System.Numerics;
using System.Text.RegularExpressions;
using FabrieBank.Common.Enums;
using FabrieBank.Services;

namespace FabrieBank
{
    public class CurrencyMenu
    {
        CurrencyService currency;
        CurrencyTable currencyTable;

        public CurrencyMenu()
        {
            currency = new CurrencyService();
            currencyTable = new CurrencyTable();
        }

        public void ShowMenu()
        {
            string choice;
            do
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine("CURRENCY RATES");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Todays Rates");
                Console.WriteLine("2. Another Days Rates");
                Console.WriteLine("3. Upper Menu");
                Console.WriteLine("==============================");
                Console.Write("Make a choice (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        TodaysRates();
                        break;
                    case "2":
                        CustomDateRates();
                        break;
                    case "3":
                        Console.WriteLine("\nPara transferinden çıkış yapıldı.\n");
                        break;
                    default:
                        Console.WriteLine("\nGeçersiz seçim. Tekrar deneyin.\n");
                        break;
                }
            } while (choice != "3");
        }

        private void TodaysRates()
        {
            CurrencyTable currencyTable = new CurrencyTable();
            currencyTable.DisplayCurrencyRatesTable(EnumDovizCinsleri.DovizCinsleri.TRY);
        }

        private void CustomDateRates()
        {
            Console.WriteLine("Which date would you like to see the exchange rate information?");
            Console.WriteLine("Please enter the date in DD / MM / YYYY format.");
            string input = Console.ReadLine();

            if (IsValidDate(input, out int day, out int month, out int year))
            {
                var baseCurrency = EnumDovizCinsleri.DovizCinsleri.TRY;

                // Fetch and display the currency rates for the custom date
                var currencyRates = currency.GetCustomDateCurrencyRates(baseCurrency, year, month, day).Result;
                currencyTable.DisplayCurrencyRatesTable(baseCurrency);
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