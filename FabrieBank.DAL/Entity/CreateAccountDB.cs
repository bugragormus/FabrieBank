using FabrieBank.DAL;
using FabrieBank.DAL.Common.DTOs;
using Npgsql;

namespace FabrieBank.DAL.Entity
{
    public class CreateAccountDB
    {
        private EAccountInfo eAccount;
        private DataAccessLayer dataAccessLayer;

        public CreateAccountDB()
        {
            eAccount = new EAccountInfo();
            dataAccessLayer = new DataAccessLayer();
        }

        public void CreateAccount(DTOAccountInfo accountInfo)
        {

            _ = eAccount.InsertAccountInfo(accountInfo);



            //Console.WriteLine($"\n'{hesapNumarasi}' Numaralı yeni hesap oluşturuldu.\n");
        }
    }
}