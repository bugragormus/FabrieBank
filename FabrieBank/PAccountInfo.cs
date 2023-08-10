using FabrieBank.DAL.Common.DTOs;
using FabrieBank.BLL.Logic;
using FabrieBank.DAL.Entity;
using System.Reflection;
using FabrieBank.DAL;

namespace FabrieBank
{
    public class PAccountInfo
    {
        private BAccount accountLogic;

        public PAccountInfo()
        {
            accountLogic = new BAccount();
        }

        /// <summary>
        /// It prints all accounts my customer_id
        /// </summary>
        /// <param name="customer"></param>
        public void AccountInfo(DTOCustomer customer)
        {
            Console.Clear();
            Console.WriteLine("******************************************************");
            Console.WriteLine("*            |                         |             *");
            Console.WriteLine("*            |      Your Accounts      |             *");
            Console.WriteLine("*            V                         V             *");
            Console.WriteLine("******************************************************\n");

            accountLogic.AccountList(customer);
        }

        /// <summary>
        /// Gets input from customer for creating accounts
        /// </summary>
        /// <param name="customer"></param>
        public void CreateAccount(DTOCustomer customer)
        {
            try
            {
                int customerId = customer.CustomerId;

                Console.WriteLine("Select currency type for account (1-TRY, 2-USD, 3-EUR, 4-GBP, 5-CHF)");
                Console.Write(">>> ");
                int currencyType = int.Parse(Console.ReadLine());
                Console.WriteLine("\nAccount Name: ");
                Console.Write(">>> ");
                string? accountName = Console.ReadLine();

                DTOAccountInfo accountInfo = new DTOAccountInfo()
                {
                    CustomerId = customerId,
                    CurrencyType = currencyType,
                    AccountName = accountName
                };

                BAccount bAccount = new BAccount();
                bAccount.CreateAccount(accountInfo);
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }
    }
}