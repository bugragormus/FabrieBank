using System;
using System.Security.Principal;
using FabrieBank.BLL.Logic;
using FabrieBank.DAL.Common.DTOs;

namespace FabrieBank
{
	public class PMenu
	{
		public PMenu()
		{
		}

		public void OpeningMenu()
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

                switch (ch)
                {
                    case "1":

                        LogIn logIn = new LogIn();
                        logIn.LogInM();
                        break;

                    case "2":

                        CreateMusteri createMusteri = new CreateMusteri();
                        createMusteri.CreateMusteriM();
                        break;

                    case "3":

                        ATMMenu();
                        break;

                    case "4":

                        DTOCustomer customer = new DTOCustomer();
                        LogIn forgot = new LogIn();
                        forgot.ForgotPassword(customer);
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

                        TransferMenu();
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

                        CreateAccount createAccount = new CreateAccount();
                        createAccount.CreateAccountM(customer);
                        break;

                    case "2":

                        AccountInfo accountInfo = new AccountInfo();
                        accountInfo.AccountInfoM(customer);
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

        public void ATMMenu()
        {
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
                        Deposit();
                        break;
                    case "2":
                        Withdraw();
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
                        TodaysRates();
                        break;
                    case "2":
                        CustomDateRates();
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
                        PersonelInfo personelInfo = new PersonelInfo();
                        personelInfo.PersonelInfoM(customer);
                        break;
                    case "2":
                        UpdatePersonelInfo(customer);
                        break;
                    case "3":
                        ChangePassword(customer);
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

        public void TransferMenu()
        {
            DTOAccountInfo dTOAccount = new DTOAccountInfo()
            {
                CustomerId = customerId
            };
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
                        TransferBetweenAccounts(accountInfos);
                        break;
                    case "2":
                        HavaleEFT(accountInfos);
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

