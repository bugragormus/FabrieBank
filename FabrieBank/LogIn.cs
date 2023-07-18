using System;
using FabrieBank.Common;
using FabrieBank.Entity;

namespace FabrieBank
{
    public class LogIn
    {
        public void LogInM()
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
            while (!int.TryParse(Console.ReadLine(), out sifre) || sifre.ToString().Length != 4)
            {
                Console.WriteLine("Invalid password. Please enter a 4-digit password:");
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
    }
}
