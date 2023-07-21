using FabrieBank.Common.Enums;
using FabrieBank.DAL;
using FabrieBank.Services;

namespace FabrieBank.Entity
{
    public class ATM
    {
        private readonly CurrencyService _currencyService;
        private DataAccessLayer dataAccessLayer;

        public ATM()
        {
            dataAccessLayer = new DataAccessLayer();
            _currencyService = new CurrencyService();
        }

        public void ParaYatirma(long hesapNo, decimal bakiye)
        {
            dataAccessLayer.Deposit(hesapNo, bakiye);
        }

        public void ParaCekme(long hesapNo, decimal bakiye)
        {
            dataAccessLayer.Withdraw(hesapNo, bakiye);
        }

        public void CurrencyTransaction(long hesapNo, decimal amount, EnumDovizCinsleri.DovizCinsleri dovizCinsi)
        {
            try
            {
                // Fetch the exchange rate from the given currency to TRY
                string currencyCode = dovizCinsi.ToString();
                decimal exchangeRateToTry = _currencyService.GetExchangeRate(currencyCode, "TRY").Result;

                // Convert the amount to Turkish Lira (TRY)
                decimal amountInTry = _currencyService.ConvertCurrency(amount, exchangeRateToTry);

                // Assuming you have a method to process the currency transaction, use the amountInTry in your logic
                ProcessCurrencyTransaction(hesapNo, amountInTry, dovizCinsi);

                // Display the result
                Console.WriteLine($"Transaction completed. Amount in TRY: {amountInTry}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing currency transaction: {ex.Message}");
            }
        }

        private void ProcessCurrencyTransaction(long hesapNo, decimal amountInTry, EnumDovizCinsleri.DovizCinsleri dovizCinsi)
        {
            // Here, you would implement the logic to perform the actual currency transaction
            // This method will vary depending on your application requirements and database structure
            // For demonstration purposes, we'll just display the transaction details
            Console.WriteLine($"Processed transaction for HesapNo: {hesapNo}");
            Console.WriteLine($"Amount in TRY: {amountInTry}");
            Console.WriteLine($"Currency: {dovizCinsi}");
        }
    }
}