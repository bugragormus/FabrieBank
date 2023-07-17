using System;
using FabrieBank.Common;
using FabrieBank.Entity;

namespace FabrieBank
{
    public class LogIn
    {
        public void LogInM()
        {
            Console.WriteLine("TCKN girin:");
            Console.Write(">>> ");
            long tckn = long.Parse(Console.ReadLine());

            Console.WriteLine("Şifre girin:");
            Console.Write(">>> ");
            int sifre = int.Parse(Console.ReadLine());

            LogInDB logInDB = new LogInDB();
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
                Console.WriteLine("*                                            *");
                Console.WriteLine("*            !!! Login Failed !!!            *");
                Console.WriteLine("*                                            *");
                Console.WriteLine("**********************************************\n");
            }
        }
    }
}
