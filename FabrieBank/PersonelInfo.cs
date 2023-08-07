using FabrieBank.DAL.Common.DTOs;

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
            Console.WriteLine("Name         :" + customer.Name);
            Console.WriteLine("Last Name    :" + customer.Lastname);
            Console.WriteLine("TCKN         :" + customer.Tckn);
            Console.WriteLine("Phone Number :" + customer.CellNo);
            Console.WriteLine("Email        :" + customer.Email);
        }
    }
}