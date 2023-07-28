using System.Reflection;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.Entity;

namespace FabrieBank
{
    public class CreateAccount
    {
        public void CreateAccountM(DTOCustomer customer)
        {
            try
            {
                int musteriId = customer.MusteriId;

                Console.WriteLine("Hangi para birimi için hesap oluşturmak istiyorsunuz? (1-TRY, 2-USD, 3-EUR, 4-GBP, 5-CHF)");
                Console.Write(">>> ");
                int dovizCinsi = int.Parse(Console.ReadLine());
                Console.WriteLine("\nHesaba bir isim verin.");
                Console.Write(">>> ");
                string? hesapAdi = Console.ReadLine();

                CreateAccountDB createAccountDB = new CreateAccountDB();
                createAccountDB.CreateAccount(musteriId, dovizCinsi, hesapAdi);
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
        }
    }
}
