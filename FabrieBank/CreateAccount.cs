using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

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

                DTOAccountInfo accountInfo = new DTOAccountInfo()
                {
                    MusteriId = musteriId,
                    DovizCins = dovizCinsi,
                    HesapAdi = hesapAdi
                };

                CreateAccountDB createAccountDB = new CreateAccountDB();
                createAccountDB.CreateAccount(accountInfo);
            }
            catch (Exception ex)
            {
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }
        }
    }
}