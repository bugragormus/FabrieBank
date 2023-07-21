using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FabrieBank.Common.DTOs;
using FabrieBank.Common.Enums;
using Newtonsoft.Json;

namespace FabrieBank.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/";

        public CurrencyService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Dictionary<string, double>> GetCurrencyRates(EnumDovizCinsleri.DovizCinsleri baseCurrency)
        {
            var currencies = Enum.GetValues(typeof(EnumDovizCinsleri.DovizCinsleri));
            var currencyRates = new Dictionary<string, double>();

            foreach (EnumDovizCinsleri.DovizCinsleri targetCurrency in currencies)
            {
                if (targetCurrency == baseCurrency)
                {
                    continue; // Skip if base currency is the same as the target currency
                }

                string url = $"{_baseUrl}{baseCurrency.ToString().ToLower()}/{targetCurrency.ToString().ToLower()}.json";
                try
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var rateDTO = JsonConvert.DeserializeObject<CurrencyRateDTO>(content);
                        currencyRates.Add(targetCurrency.ToString(), rateDTO.Rate);
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
            }

            return currencyRates;
        }
    }
}
