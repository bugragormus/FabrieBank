using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FabrieBank
{
    public class CreateMusteri
    {
        public void CreateMusteriM()
        {
            Console.WriteLine("\n==============================================");
            Console.Write("Customer Name: ");
            string? musteriAd = Console.ReadLine();

            Console.WriteLine("\n==============================================");
            Console.Write("Customer Lastname: ");
            string? musteriSoyad = Console.ReadLine();

            Console.WriteLine("\n==============================================");
            Console.Write("Customer TCKN: ");
            long musteriTCKN = Convert.ToInt64(Console.ReadLine());

            Console.WriteLine("\n==============================================");
            Console.Write("Customer Password: ");
            int musteriSifre = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\n==============================================");
            Console.Write("Customer Cell Number: ");
            long musteriTelNo = Convert.ToInt64(Console.ReadLine());

            Console.WriteLine("\n==============================================");
            Console.Write("Customer Email: ");
            string? musteriEmail = Console.ReadLine();

            // Retrieve other necessary inputs from the user

            // Create an instance of CreateCustomerDB and call the CreateCustomer method
            FabrieBank.Entity.CreateCustomerDB createCustomerDB = new FabrieBank.Entity.CreateCustomerDB();
            createCustomerDB.CreateCustomer(musteriAd, musteriSoyad, musteriTCKN, musteriSifre, musteriTelNo, musteriEmail);
            Console.Clear();
        }
    }
}
