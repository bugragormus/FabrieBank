using FabrieBank.DAL.Common.DTOs;
using FabrieBank.BLL.Logic;
using FabrieBank.DAL.Entity;

namespace FabrieBank
{
    class Program
    {
        private static void Main()
        {
            string? ch;

            for (; ; )
            {
                Console.WriteLine("=====================================================================================");
                Console.WriteLine("                                  Welcome To FabrieBank!                             ");
                Console.WriteLine("=====================================================================================");
                Console.WriteLine("1.Login           2.New Customer         3.ATM         4.Forgot Password       5.Exit");
                Console.WriteLine("=====================================================================================");
                Console.Write(">>> ");
                ch = Console.ReadLine();

                DTOCustomer customer = new DTOCustomer();

                switch (ch)
                {
                    case "1":

                        PCustomer pCustomer = new PCustomer(customer.CustomerId);
                        pCustomer.LogIn();
                        break;

                    case "2":

                        PCustomer pCustomer1 = new PCustomer(customer.CustomerId);
                        pCustomer1.CreateCustomer();
                        break;

                    case "3":

                        ATMMenu();
                        break;

                    case "4":

                        PCustomer pCustomer2 = new PCustomer(customer.CustomerId);
                        pCustomer2.ForgotPassword(customer);
                        break;

                    case "5":

                        Environment.Exit(0);
                        break;

                    default:

                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }

        public void MainMenu(DTOCustomer customer)
        {
            string? ch;

            for (; ; )
            {
                Console.WriteLine("=============================================================================================");
                Console.WriteLine("                                         MAIN MENU                                           ");
                Console.WriteLine("=============================================================================================");
                Console.WriteLine("1.User Operations    2.Bank Accounts     3.Money Transfers     4. Currency Rates     5.Exit  ");
                Console.WriteLine("=============================================================================================");
                Console.Write(">>> ");
                ch = Console.ReadLine();
                Console.Clear();

                switch (ch)
                {
                    case "1":

                        CustomerMenu(customer);
                        break;

                    case "2":

                        AccountMenu(customer);
                        break;

                    case "3":

                        TransferMenu(customer.CustomerId);
                        break;

                    case "4":

                        CurrencyMenu();
                        break;

                    case "5":

                        Environment.Exit(0);
                        break;

                    default:

                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }

        public void AccountMenu(DTOCustomer customer)
        {
            string? ch;

            for (; ; )
            {
                Console.WriteLine("=====================================================================");
                Console.WriteLine("1.New Account    2.Show Accounts     3.Delete Account    4.Upper Menu");
                Console.WriteLine("=====================================================================");
                Console.Write(">>> ");
                ch = Console.ReadLine();
                switch (ch)
                {
                    case "1":

                        PAccountInfo pAccountInfo1 = new PAccountInfo();
                        pAccountInfo1.CreateAccount(customer);
                        break;

                    case "2":

                        PAccountInfo pAccountInfo = new PAccountInfo();
                        pAccountInfo.AccountInfo(customer);
                        break;

                    case "3":

                        BAccount accountLogic = new BAccount();
                        accountLogic.DeleteAccount(customer);
                        break;

                    case "4":

                        MainMenu(customer);
                        break;

                    default:

                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }

        public static void ATMMenu()
        {
            DTOCustomer customer = new DTOCustomer();
            string choice;

            do
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine("ATM TRANSACTIONS");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Deposit");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. Back to Main Menu");
                Console.WriteLine("==============================");
                Console.Write("Make your choice (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PTransaction pTransaction = new PTransaction(customer.CustomerId);
                        pTransaction.Deposit();
                        break;
                    case "2":
                        PTransaction pTransaction1 = new PTransaction(customer.CustomerId);
                        pTransaction1.Withdraw();
                        break;
                    case "3":
                        Console.WriteLine("Checked out from ATM.");
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Try again.");
                        break;
                }
            } while (choice != "3");
        }

        public void CurrencyMenu()
        {
            string choice;
            do
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine("CURRENCY RATES");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Todays Rates");
                Console.WriteLine("2. Another Days Rates");
                Console.WriteLine("3. Upper Menu");
                Console.WriteLine("==============================");
                Console.Write("Make a choice (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PCurrency pCurrency = new PCurrency();
                        pCurrency.TodaysRates();
                        break;
                    case "2":
                        PCurrency pCurrency1 = new PCurrency();
                        pCurrency1.CustomDateRates();
                        break;
                    case "3":
                        Console.WriteLine("\nExited from Currency Rates.\n");
                        break;
                    default:
                        Console.WriteLine("\nInvalid selection. Try again.\n");
                        break;
                }
            } while (choice != "3");
        }

        public void CustomerMenu(DTOCustomer customer)
        {
            string choice;

            do
            {
                Console.WriteLine("==============================");
                Console.WriteLine("USER OPERATIONS");
                Console.WriteLine("==============================");
                Console.WriteLine("1. View Personal Information");
                Console.WriteLine("2. Update Personal Information");
                Console.WriteLine("3. Change Password");
                Console.WriteLine("4. Back to Main Menu");
                Console.WriteLine("==============================");
                Console.Write("Make your pick (1-4): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PCustomer pCustomer2 = new PCustomer(customer.CustomerId);
                        pCustomer2.PersonelInfo(customer);
                        break;
                    case "2":
                        PCustomer pCustomer1 = new PCustomer(customer.CustomerId);
                        pCustomer1.UpdatePersonelInfo(customer);
                        break;
                    case "3":
                        PCustomer pCustomer = new PCustomer(customer.CustomerId);
                        pCustomer.ChangePassword(customer);
                        break;
                    case "4":
                        Console.WriteLine("\nExited from User Operations.\n");
                        break;
                    default:
                        Console.WriteLine("\nInvalid selection. Try again.\n");
                        break;
                }
            } while (choice != "4");
        }

        public void TransferMenu(int customerId)
        {
            DTOAccountInfo dTOAccount = new DTOAccountInfo()
            {
                CustomerId = customerId
            };

            EAccountInfo eAccount = new EAccountInfo();
            List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccount);

            string choice;
            do
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine("MONEY TRANSFERS");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Transfer Between Accounts");
                Console.WriteLine("2. To Another Account Havale/EFT");
                Console.WriteLine("3. Back to Main Menu");
                Console.WriteLine("==============================");
                Console.Write("Make your choice (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":

                        PTransaction pTransaction = new PTransaction(customerId);
                        pTransaction.TransferBetweenAccounts(accountInfos);
                        break;

                    case "2":

                        PTransaction pTransaction1 = new PTransaction(customerId);
                        pTransaction1.HavaleEFT(accountInfos);
                        break;

                    case "3":

                        Console.WriteLine("Exited from money transfers");
                        break;

                    default:

                        Console.WriteLine("Invalid selection. Try again.");
                        break;
                }
            } while (choice != "3");
        }
    }
}