using System;
using FabrieBank.Common;
using FabrieBank.Entity;

namespace FabrieBank
{
    public class PersonelInfo
    {
        public void PersonelInfoM(DTOCustomer customer)
        {
            Console.Clear();
            Console.WriteLine("******************************************************");
            Console.WriteLine("*            |                         |             *");
            Console.WriteLine("*            |   Customer Information  |             *");
            Console.WriteLine("*            V                         V             *");
            Console.WriteLine("******************************************************\n");
            Console.WriteLine("Name         :" + customer.Ad);
            Console.WriteLine("Last Name    :" + customer.Soyad);
            Console.WriteLine("TCKN         :" + customer.Tckn);
            Console.WriteLine("Phone Number :" + customer.TelNo);
            Console.WriteLine("Email        :" + customer.Email);
        }
    }
}

