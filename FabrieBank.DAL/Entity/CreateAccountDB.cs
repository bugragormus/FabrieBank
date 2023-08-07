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

            int numericValue = accountInfo.CurrencyType;
            string currencyName = Enum.GetName(typeof(EnumCurrencyTypes.CurrencyTypes), numericValue);

            if (accountInfo.AccountName != "")
            {
                Console.WriteLine($"\n'{accountInfo.AccountName}' named new '{currencyName}' account has been created.\n");
            }
            else
            {
                Console.WriteLine($"\nNew '{currencyName}' account has been created.\n");
            }
        }
    }
}