using FabrieBank.DAL.Common.DTOs;

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

            Console.WriteLine($"\n'{accountInfo.HesapNo}' Numaralı yeni hesap oluşturuldu.\n");
        }
    }
}