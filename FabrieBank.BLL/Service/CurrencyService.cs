using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using FabrieBank.Common.DTOs;
using FabrieBank.Common.Enums;

namespace FabrieBank.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://www.tcmb.gov.tr/kurlar/today.xml";

        public CurrencyService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Dictionary<string, double>> GetCurrencyRates(EnumDovizCinsleri.DovizCinsleri baseCurrency)
        {
            var currencies = Enum.GetValues(typeof(EnumDovizCinsleri.DovizCinsleri));
            var currencyRates = new Dictionary<string, double>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(content);

                    foreach (EnumDovizCinsleri.DovizCinsleri targetCurrency in currencies)
                    {
                        if (targetCurrency == baseCurrency)
                        {
                            continue; // Skip if base currency is the same as the target currency
                        }

                        string currencyCode = targetCurrency.ToString();
                        string xpath = $"Tarih_Date/Currency[@Kod='{currencyCode}']/BanknoteSelling";
                        XmlNode node = xmlDoc.SelectSingleNode(xpath);
                        if (node != null)
                        {
                            double rate = Convert.ToDouble(node.InnerText, CultureInfo.GetCultureInfo("en-US"));
                            currencyRates.Add(currencyCode, rate);
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
                // Handle exceptions if needed
                Console.WriteLine($"Error: {ex.Message}");
            }

            return currencyRates;
        }
    }
}
