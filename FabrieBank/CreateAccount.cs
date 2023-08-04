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
                int customerId = customer.CustomerId;

                Console.WriteLine("Select currency type for account (1-TRY, 2-USD, 3-EUR, 4-GBP, 5-CHF)");
                Console.Write(">>> ");
                int currencyType = int.Parse(Console.ReadLine());
                Console.WriteLine("\nAccount Name: .");
                Console.Write(">>> ");
                string? accountName = Console.ReadLine();

                DTOAccountInfo accountInfo = new DTOAccountInfo()
                {
                    CustomerId = customerId,
                    CurrencyType = currencyType,
                    AccountName = accountName
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