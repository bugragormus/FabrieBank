using FabrieBank.DAL.Common.DTOs;
using FabrieBank.BLL.Logic;

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

                        ATMMenu atmMenu = new ATMMenu();
                        atmMenu.ShowMenu();
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
            String? ch;

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

                        CustomerMenu customerMenu = new CustomerMenu(customer.MusteriId);
                        customerMenu.ShowMenu(customer);
                        break;

                    case "2":

                        AccountMenu(customer);
                        break;

                    case "3":

                        TransferMenu transferMenu = new TransferMenu(customer.MusteriId);
                        transferMenu.ShowMenu();
                        break;

                    case "4":

                        CurrencyMenu currencyMenu = new CurrencyMenu();
                        currencyMenu.ShowMenu();
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
            String? ch;

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
                        accountLogic.HesapSil(customer);
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
    }
}