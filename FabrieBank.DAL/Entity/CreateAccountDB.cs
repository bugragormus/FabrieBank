using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;

namespace FabrieBank.DAL.Entity
{
    public class CreateAccountDB
    {
        private EAccountInfo eAccount;

        public CreateAccountDB()
        {
            eAccount = new EAccountInfo();
        }

        public void CreateAccount(DTOAccountInfo accountInfo)
        {
            _ = eAccount.InsertAccountInfo(accountInfo);

            int numericValue = accountInfo.DovizCins;
            string currencyName = Enum.GetName(typeof(EnumDovizCinsleri.DovizCinsleri), numericValue);

            if (accountInfo.HesapAdi != "")
            {
                Console.WriteLine($"\n'{accountInfo.HesapAdi}' isimli yeni '{currencyName}' hesabı oluşturuldu.\n");
            }
            else
            {
                Console.WriteLine($"\nYeni '{currencyName}' hesabı oluşturuldu.\n");
            }
        }
    }
}