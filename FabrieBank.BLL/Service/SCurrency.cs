using System.Globalization;
using System.Reflection;
using System.Xml;
using FabrieBank.DAL;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;
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

        public async Task<Dictionary<string, DTOCurrencyRate>> GetTodaysCurrencyRates(EnumCurrencyTypes.CurrencyTypes baseCurrency)
        {
            var currencies = Enum.GetValues(typeof(EnumCurrencyTypes.CurrencyTypes));
            var currencyRates = new Dictionary<string, DTOCurrencyRate>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(content);

                    foreach (EnumCurrencyTypes.CurrencyTypes targetCurrency in currencies)
                    {
                        if (targetCurrency == baseCurrency)
                        {
                            continue; // Skip if base currency is the same as the target currency
                        }

                        EnumCurrencyTypes.CurrencyTypes currencyCode = targetCurrency;
                        string xpath = $"Tarih_Date/Currency[@Kod='{currencyCode}']";

                        XmlNode node = xmlDoc.SelectSingleNode(xpath);
                        if (node != null)
                        {
                            DTOCurrencyRate currency = new DTOCurrencyRate();
                            currency.CurrencyCode = currencyCode;

                            double banknoteBuyingRate = Convert.ToDouble(node["BanknoteBuying"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.BanknoteBuyingRate = (decimal)banknoteBuyingRate - ((decimal)banknoteBuyingRate * 0.05M);

                            double banknoteSellingRate = Convert.ToDouble(node["BanknoteSelling"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.BanknoteSellingRate = (decimal)banknoteSellingRate + ((decimal)banknoteSellingRate * 0.05M);

                            double forexBuyingRate = Convert.ToDouble(node["ForexBuying"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.ForexBuyingRate = (decimal)forexBuyingRate - ((decimal)forexBuyingRate * 0.05M);

                            double forexSellingRate = Convert.ToDouble(node["ForexSelling"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.ForexSellingRate = (decimal)forexSellingRate + ((decimal)forexBuyingRate * 0.05M);

                            currencyRates.Add(currencyCode.ToString(), currency);
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
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }

            return currencyRates;
        }

        public async Task<Dictionary<string, DTOCurrencyRate>> GetCustomDateCurrencyRates(EnumCurrencyTypes.CurrencyTypes baseCurrency, int year, int month, int day)
        {
            var currencies = Enum.GetValues(typeof(EnumCurrencyTypes.CurrencyTypes));
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

                    foreach (EnumCurrencyTypes.CurrencyTypes targetCurrency in currencies)
                    {
                        if (targetCurrency == baseCurrency)
                        {
                            continue; // Skip if base currency is the same as the target currency
                        }

                        EnumCurrencyTypes.CurrencyTypes currencyCode = targetCurrency;
                        string xpath = $"Tarih_Date/Currency[@Kod='{currencyCode}']";

                        XmlNode node = xmlDoc.SelectSingleNode(xpath);
                        if (node != null)
                        {
                            DTOCurrencyRate currency = new DTOCurrencyRate();
                            currency.CurrencyCode = currencyCode;

                            double banknoteBuyingRate = Convert.ToDouble(node["BanknoteBuying"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.BanknoteBuyingRate = (decimal)banknoteBuyingRate;

                            double banknoteSellingRate = Convert.ToDouble(node["BanknoteSelling"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.BanknoteSellingRate = (decimal)banknoteSellingRate;

                            double forexBuyingRate = Convert.ToDouble(node["ForexBuying"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.ForexBuyingRate = (decimal)forexBuyingRate;

                            double forexSellingRate = Convert.ToDouble(node["ForexSelling"].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currency.ForexSellingRate = (decimal)forexSellingRate;

                            currencyRates.Add(currencyCode.ToString(), currency);
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
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

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

        public DTOCurrency ReadCurrency(DTOCurrency accountInfo)
        {
            ECurrency eCurrency = new ECurrency();
            return eCurrency.ReadCurrency(accountInfo);
        }

        public bool InsertCurrency(DTOCurrency accountInfo)
        {
            ECurrency eCurrency = new ECurrency();
            return eCurrency.InsertCurrency(accountInfo);
        }

        public bool UpdateCurrency(DTOCurrency accountInfo)
        {
            ECurrency eCurrency = new ECurrency();
            return eCurrency.UpdateCurrency(accountInfo);
        }

        public bool DeleteCurrency(DTOCurrency accountInfo)
        {
            ECurrency eCurrency = new ECurrency();
            return eCurrency.DeleteCurrency(accountInfo);
        }
    }
}