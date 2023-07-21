using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FabrieBank.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies";

        public CurrencyService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<decimal> GetExchangeRate(string sourceCurrency, string targetCurrency)
        {
            try
            {
                string apiUrl = $"{BaseUrl}/{sourceCurrency.ToLower()}/{targetCurrency.ToLower()}.json";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var currencyRate = JsonConvert.DeserializeObject<DTOCurrency>(content);
                    return currencyRate.Rate;
                }
            }
            catch (Exception ex)
            {
                // Handle API request errors here
                Console.WriteLine($"Error fetching currency rates: {ex.Message}");
            }

            return 0;
        }

        // Currency conversion logic
        public decimal ConvertCurrency(decimal amount, decimal exchangeRate)
        {
            return amount * exchangeRate;
        }

        // DTO class to represent the API response
        private class DTOCurrency
        {
            [JsonProperty("TRY")]
            public decimal Rate { get; set; }
        }
    }
}
