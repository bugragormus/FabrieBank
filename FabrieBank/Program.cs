using System;
using FabrieBank.Common;
using FabrieBank.Entity;

namespace FabrieBank
{
    class Program
    {
        static void Main(string[] args)
        {
            String? ch;

            for (; ; )
            {
                Console.WriteLine("==============================================");
                Console.WriteLine("\t     Welcome To FabrieBank!");
                Console.WriteLine("==============================================");
                Console.WriteLine("1.Login\t\t2.New Customer\t\t3.Exit");
                Console.WriteLine("==============================================");
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

                        Environment.Exit(0);
                        break;

                    default:

                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }

        public void Menu(DTOCustomer customer)
        {
            String? ch;

            //Main menu for loged in users
            for (; ; )
            {
                Console.WriteLine("===================================================");
                Console.WriteLine("                      MAIN MENU");
                Console.WriteLine("===================================================");
                Console.WriteLine("1.Personal Info  2.ATM   3.Bank Accounts     4.Exit");
                Console.WriteLine("===================================================");
                Console.Write(">>> ");
                ch = Console.ReadLine();

                switch (ch)
                {
                    case "1":

                        PersonelInfo personelInfo = new PersonelInfo();
                        personelInfo.PersonelInfoM(customer);
                        break;

                    case "2":

                        ATMMenu atmMenu = new ATMMenu(customer.MusteriId);
                        atmMenu.ShowMenu();
                        break;

                    case "3":

                        AccountMenu(customer);
                        break;

                    case "4":

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
                Console.WriteLine("==========================================================");
                Console.WriteLine("1.New Account    2.Show Accounts    3.Upper Menu    4.Exit");
                Console.WriteLine("==========================================================");
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

                        Menu(customer);
                        break;

                    case "4":

                        Environment.Exit(0);
                        break;

                    default:

                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }
    }
}