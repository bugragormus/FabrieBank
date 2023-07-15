using System;
using FabrieBank.Common;
using FabrieBank.Entity;

namespace FabrieBank
{
    public class CreateAccount
    {
        public void CreateAccountM(DTOCustomer customer)
        {
            int musteriId = customer.MusteriId;

            Console.WriteLine("Hangi para birimi için hesap oluşturmak istiyorsunuz? (1-TL, 2-USD, 3-EUR, 4-GAU, 5-XAG)");
            Console.Write(">>> ");
            int dovizCinsi = int.Parse(Console.ReadLine());
            Console.WriteLine("\nHesaba bir isim verin.");
            Console.Write(">>> ");
            string? hesapAdi = Console.ReadLine();

            CreateAccountDB createAccountDB = new CreateAccountDB();
            createAccountDB.CreateAccount(musteriId, dovizCinsi, hesapAdi);
        }
    }
}
