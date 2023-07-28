using System.Globalization;
using System.Reflection;
using System.Xml;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank.Services
{
    public class SCurrency : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://www.tcmb.gov.tr/kurlar/today.xml"; 

        public SCurrency()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Dictionary<string, DTOCurrencyRate>> GetTodaysCurrencyRates(int baseCurrency)
        {
            var currencies = Enum.GetValues(typeof(int));
            var currencyRates = new Dictionary<string, DTOCurrencyRate>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(content);

                    foreach (int targetCurrency in currencies)
                    {
                        if (targetCurrency == baseCurrency)
                        {
                            continue; // Skip if base currency is the same as the target currency
                        }

                        string currencyCode = targetCurrency.ToString();
                        string xpath = $"Tarih_Date/Currency[@Kod='{currencyCode}']";

                        XmlNode node = xmlDoc.SelectSingleNode(xpath);
                        if (node != null)
                        {
                            DTOCurrencyRate currency = new DTOCurrencyRate();
                            currency.CurrencyCode = currencyCode;

                            double banknoteBuyingRate = Convert.ToDouble(node["BanknoteBuying"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.BanknoteBuyingRate = banknoteBuyingRate;

                            double banknoteSellingRate = Convert.ToDouble(node["BanknoteSelling"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.BanknoteSellingRate = banknoteSellingRate;

                            double forexBuyingRate = Convert.ToDouble(node["ForexBuying"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.ForexBuyingRate = forexBuyingRate;

                            double forexSellingRate = Convert.ToDouble(node["ForexSelling"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.ForexSellingRate = forexSellingRate;

                            currencyRates.Add(currencyCode, currency);
                        }
                        else
                        {
                            // Handle missing currency data if needed
                            Console.WriteLine($"Currency data not found for {currencyCode}.");
                        }
                    }
                }
                else
                {
                    // Handle API response errors if needed
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
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

            return currencyRates;
        }

        public async Task<Dictionary<string, DTOCurrencyRate>> GetCustomDateCurrencyRates(int baseCurrency, int year, int month, int day)
        {
            var currencies = Enum.GetValues(typeof(int));
            var currencyRates = new Dictionary<string, DTOCurrencyRate>();

            try
            {
                string url = $"https://www.tcmb.gov.tr/kurlar/{year:0000}{month:00}/{day:00}{month:00}{year:0000}.xml";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(content);

                    foreach (int targetCurrency in currencies)
                    {
                        if (targetCurrency == baseCurrency)
                        {
                            continue; // Skip if base currency is the same as the target currency
                        }

                        string currencyCode = targetCurrency.ToString();
                        string xpath = $"Tarih_Date/Currency[@Kod='{currencyCode}']";

                        XmlNode node = xmlDoc.SelectSingleNode(xpath);
                        if (node != null)
                        {
                            DTOCurrencyRate currency = new DTOCurrencyRate();
                            currency.CurrencyCode = currencyCode;

                            double banknoteBuyingRate = Convert.ToDouble(node["BanknoteBuying"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.BanknoteBuyingRate = banknoteBuyingRate;

                            double banknoteSellingRate = Convert.ToDouble(node["BanknoteSelling"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.BanknoteSellingRate = banknoteSellingRate;

                            double forexBuyingRate = Convert.ToDouble(node["ForexBuying"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.ForexBuyingRate = forexBuyingRate;

                            double forexSellingRate = Convert.ToDouble(node["ForexSelling"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.ForexSellingRate = forexSellingRate;

                            currencyRates.Add(currencyCode, currency);
                        }
                        else
                        {
                            // Handle missing currency data if needed
                            Console.WriteLine($"Currency data not found for {currencyCode}.");
                        }
                    }
                }
                else
                {
                    // Handle API response errors if needed
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
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

            return currencyRates;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public List<DTOCurrency> ReadListCurrency(DTOCurrency currency)
        {
            ECurrency eCurrency = new ECurrency();
            return eCurrency.ReadListCurrency(currency);
        }
    }
}