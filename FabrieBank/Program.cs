﻿using FabrieBank.Common;
using FabrieBank.BLL;
using FabrieBank.Entity;
using FabrieBank.Common.Enums;

namespace FabrieBank
{
    class Program
    {
        static void Main(string[] args)
        {
            String? ch;

            for (; ; )
            {
                Console.WriteLine("======================================================================");
                Console.WriteLine("                          Welcome To FabrieBank!                      ");
                Console.WriteLine("======================================================================");
                Console.WriteLine("1.Login           2.New Customer        3.Forgot Password       4.Exit");
                Console.WriteLine("======================================================================");
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

                        LogIn forgot = new LogIn();
                        forgot.ForgotPassword();
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

        public void Menu(DTOCustomer customer)
        {
            String? ch;

            //Main menu for loged in users
            for (; ; )
            {
                Console.WriteLine("=========================================================================");
                Console.WriteLine("                                 MAIN MENU                               ");
                Console.WriteLine("=========================================================================");
                Console.WriteLine("1.User Operations  2.ATM   3.Bank Accounts   4.Money Transfers     5.Exit");
                Console.WriteLine("=========================================================================");
                Console.Write(">>> ");
                ch = Console.ReadLine();
                Console.Clear();

                switch (ch)
                {
                    case "1":

                        //CustomerMenu customerMenu = new CustomerMenu(customer.MusteriId);
                        //customerMenu.ShowMenu(customer);

                        ATM atm = new ATM();
                        decimal amountInEur = 100;
                        EnumDovizCinsleri.DovizCinsleri dovizCinsi = EnumDovizCinsleri.DovizCinsleri.EUR;
                        long hesapNo = 123456789; // Replace with the actual account number

                        atm.CurrencyTransaction(hesapNo, amountInEur, dovizCinsi);
                        break;

                    case "2":

                        ATMMenu atmMenu = new ATMMenu(customer.MusteriId);
                        atmMenu.ShowMenu();
                        break;

                    case "3":

                        AccountMenu(customer);
                        break;

                    case "4":

                        TransferMenu transferMenu = new TransferMenu(customer.MusteriId);
                        transferMenu.ShowMenu();
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

                        AccountLogic accountLogic = new AccountLogic();
                        accountLogic.HesapSil();
                        break;

                    case "4":

                        Menu(customer);
                        break;

                    default:

                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }
    }
}