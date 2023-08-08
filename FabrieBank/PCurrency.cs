using System.Text.RegularExpressions;
using FabrieBank.BLL.Logic;
using FabrieBank.BLL.Service;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;
using FabrieBank.DAL.Entity;
using FabrieBank.Services;

namespace FabrieBank
{
    public class PCurrency
    {
        private readonly SCurrency currency;
        private EnumCurrencyTypes.CurrencyTypes baseCurrency;
        private EAccountInfo eAccount;
        private BAccount bAccount;

        public PCurrency()
        {
            currency = new SCurrency();
            baseCurrency = EnumCurrencyTypes.CurrencyTypes.TRY;//TRY
            eAccount = new EAccountInfo();
            bAccount = new BAccount();
        }

        public void TodaysRates()
        {
            try
            {
                var baseCurrency = EnumCurrencyTypes.CurrencyTypes.TRY;//TRY
                var currencyRates = currency.GetTodaysCurrencyRates(baseCurrency).Result;
                DisplayCurrencyRatesTable(baseCurrency, currencyRates);
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
                Console.WriteLine("Please enter the date in DD.MM.YYYY format.");
                string input = Console.ReadLine();

                if (IsValidDate(input, out int day, out int month, out int year))
                {
                    var baseCurrency = EnumCurrencyTypes.CurrencyTypes.TRY;//TRY

                    // Fetch the currency rates for the custom date
                    var currencyRates = currency.GetCustomDateCurrencyRates(baseCurrency, year, month, day).Result;

                    // Check if currency rates are available for the custom date
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
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

        public List<DTOCurrencyRate> GetCurrencyRates(Dictionary<string, DTOCurrencyRate> currencyRates)
        {
            List<DTOCurrencyRate> dtoCurrencyRates = new List<DTOCurrencyRate>();

            foreach (var rate in currencyRates)
            {
                DTOCurrencyRate dtoCurrencyRate = new DTOCurrencyRate
                {
                    CurrencyCode = rate.Key,
                    ForexBuyingRate = rate.Value.ForexBuyingRate,
                    ForexSellingRate = rate.Value.ForexSellingRate,
                    BanknoteBuyingRate = rate.Value.BanknoteBuyingRate,
                    BanknoteSellingRate = rate.Value.BanknoteSellingRate
                };

                dtoCurrencyRates.Add(dtoCurrencyRate);
            }

            return dtoCurrencyRates;
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

            // Tarih geçerlilik kontrolü
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
                // Şubat ayı için özel kontrol
                if (month == 2)
                {
                    // Şubat ayı 28 veya 29 gün çekebilir (artık yıl kontrolü yapmalıyız)
                    bool isLeapYear = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
                    if (day > 29 || (day == 29 && !isLeapYear))
                    {
                        Console.WriteLine("Geçersiz gün! Şubat için geçersiz gün değeri.");
                        return false;
                    }
                }
                // Diğer aylar için gün sayısı kontrolü
                else if (day > 30 && (month == 4 || month == 6 || month == 9 || month == 11))
                {
                    Console.WriteLine("Geçersiz gün! Bu ay için geçersiz gün değeri.");
                    return false;
                }
            }
            return true;
        }

        //public void GetRate()
        //{
        //    try
        //    {
        //        var baseCurrency = EnumCurrencyTypes.CurrencyTypes.TRY;//TRY
        //        var currencyRates = currency.GetTodaysCurrencyRates(baseCurrency).Result;
        //        List<DTOCurrencyRate> dtoCurrencyRates = GetCurrencyRates(currencyRates);
        //    }
        //    catch (Exception ex)
        //    {
        //        EErrorLogger errorLogger = new EErrorLogger();
        //        errorLogger.LogAndHandleError(ex);
        //    }
        //}

        public void ExchangeBuying(DTOCustomer customer)
        {
            var currencyRates = currency.GetTodaysCurrencyRates(baseCurrency).Result;
            List<DTOCurrencyRate> dTOCurrencyRates = GetCurrencyRates(currencyRates);

            if (dTOCurrencyRates.Count > 0)
            {
                Console.WriteLine("\nBanknote Selling Rate for;  ");
                for (int i = 0; i < dTOCurrencyRates.Count; i++)
                {
                    decimal banknoteSellingRate = dTOCurrencyRates[i].BanknoteSellingRate;
                    string currencyType = dTOCurrencyRates[i].CurrencyCode;
                    Console.WriteLine($"\n[{i}] {currencyType}: {banknoteSellingRate}");
                }
                Console.WriteLine("What type of currency would you like to trade?");
                Console.Write(">>> ");
                int exchange = int.Parse(Console.ReadLine());
                decimal exchangeRate = dTOCurrencyRates[exchange].BanknoteSellingRate;
                string exchangeType = dTOCurrencyRates[exchange].CurrencyCode;

                DTOAccountInfo dTOAccounts = new DTOAccountInfo()
                {
                    CustomerId = customer.CustomerId,
                    CurrencyType = (int)baseCurrency
                };

                List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccounts);

                Console.WriteLine("\nFrom which account would you like to withdraw the money?\n");
                bAccount.PrintAccountList(accountInfos);

                Console.Write("Account Index: ");
                int withdrawAccIndex = int.Parse(Console.ReadLine());

                if (withdrawAccIndex >= 0 && withdrawAccIndex < accountInfos.Count)
                {
                    long withdrawAccNo = accountInfos[withdrawAccIndex].AccountNo;
                    decimal withdrawAccBalance = accountInfos[withdrawAccIndex].Balance;

                    DTOAccountInfo dTOAccount = new DTOAccountInfo()
                    {
                        CustomerId = customer.CustomerId,
                        CurrencyType = 2
                    };

                    List<DTOAccountInfo> accountInfo = eAccount.ReadListAccountInfo(dTOAccount);

                    Console.WriteLine("\nFrom which account would you like to withdraw the money?\n");
                    bAccount.PrintAccountList(accountInfo);

                    Console.Write("Account Index: ");
                    int depositAccIndex = int.Parse(Console.ReadLine());

                    if (depositAccIndex >= 0 && depositAccIndex < accountInfos.Count)
                    {
                        long depositAccNo = accountInfos[depositAccIndex].AccountNo;
                        decimal depositAccBalance = accountInfos[depositAccIndex].Balance;

                        Console.WriteLine("\nkaç para?\n");
                        decimal amount = int.Parse(Console.ReadLine());

                        decimal tl = amount * exchangeRate;
                        Console.WriteLine(tl);
                    }
                    else
                    {
                        Console.WriteLine("No.");
                    }

                }
                else
                {
                    Console.WriteLine("No currency rates available.");
                }
            }
        }
        public void ExchangeSelling(DTOCustomer customer)
        {

        }
    }
}