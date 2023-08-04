﻿using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank
{
    public class LogIn
    {
        private ErrorLoggerDB errorLogger;
        LogInDB loginDB = new LogInDB();
        bool loggedIn = false;

        public LogIn()
        {
            errorLogger = new ErrorLoggerDB();
        }

        public void LogInM()
        {
            try
            {
                LogInDB logInDB = new LogInDB();

                Console.WriteLine("TCKN girin:");
                Console.Write(">>> ");
                long tckn;
                while (!long.TryParse(Console.ReadLine(), out tckn) || tckn.ToString().Length != 11)
                {
                    Console.WriteLine("Invalid TCKN. Please enter a 11-digit TCKN:");
                }

                Console.WriteLine("Şifre girin:");
                Console.Write(">>> ");
                int sifre;
                while (!int.TryParse(GetMaskedInput(), out sifre) || sifre.ToString().Length != 4)
                {
                    Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                }

                static string GetMaskedInput()
                {
                    string input = "";
                    ConsoleKeyInfo keyInfo;

                    do
                    {
                        keyInfo = Console.ReadKey(true);

                        if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
                        {
                            input += keyInfo.KeyChar;
                            Console.Write("*");
                        }
                        else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                        {
                            input = input.Substring(0, input.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                    while (keyInfo.Key != ConsoleKey.Enter);

                    return input;
                }

                DTOCustomer customer = logInDB.LogIn(tckn, sifre);

                if (customer != null)
                {
                    Console.Clear();
                    Console.WriteLine("******************************************************");
                    Console.WriteLine("*            |                         |             *");
                    Console.WriteLine("*            |    Login Successfull    |             *");
                    Console.WriteLine("*            V                         V             *");
                    Console.WriteLine("******************************************************\n");

                    Program program = new Program();
                    program.Menu(customer);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("**********************************************");
                    Console.WriteLine("*            !!! Login Failed !!!            *");
                    Console.WriteLine("*                                            *");
                    Console.WriteLine("*         !Wrong TCKN or Password!           *");
                    Console.WriteLine("**********************************************\n");
                }
            }
            catch (Exception ex)
            {
                errorLogger.LogAndHandleError(ex);
            }
        }

        public void ForgotPassword(DTOCustomer customer)
        {
            Console.Write("TCKN: ");
            long tckn;
            while (!long.TryParse(Console.ReadLine(), out tckn) || tckn.ToString().Length != 11)
            {
                Console.WriteLine("Invalid TCKN. Please enter a 11-digit TCKN:");
            }

            Console.Write("Email: ");
            string email = Console.ReadLine();

            while (!CreateMusteri.IsValidEmail(email))
            {
                Console.WriteLine("Invalid email address. Please enter a valid email address:");
                email = Console.ReadLine();
            }

            customer.Tckn = tckn;

            loginDB.ForgotPassword(customer, email);
        }
    }
}
