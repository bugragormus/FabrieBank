using FabrieBank.Admin.DAL.DTO;
using FabrieBank.Admin.DAL.Entity;
using FabrieBank.BLL.Service;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank.Admin.BLL.Logic
{
    public class BAdmin
    {
        private EAdmin eAdmin;
        private ECustomer eCustomer;

        public BAdmin()
        {
            eAdmin = new EAdmin();
            eCustomer = new ECustomer();
        }

        /// <summary>
        /// Admin login logic
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public DTOAdmin LogIn(string nickname, string password)
        {
            DTOAdmin admin = eAdmin.ReadAdmin(new DTOAdmin { Nickname = nickname });

            if (admin != null && admin.Password == password)
            {
                return admin;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Customer status update logic
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool CustomerStatusUpdate(DTOCustomer customer)
        {
            eCustomer.UpdateCustomer(customer);
            return true;
        }

        /// <summary>
        /// Read list customer logic
        /// </summary>
        /// <param name="customer"></param>
        public void ListCustomers(DTOCustomer customer)
        {
            Console.WriteLine("\n");
            if (customer.Tckn != 0)
            {
                DTOCustomer dTOCustomer = eCustomer.ReadCustomer(customer);

                    Console.WriteLine($"ID      : {dTOCustomer.CustomerId}");
                    Console.WriteLine($"Name    : {dTOCustomer.Name}");
                    Console.WriteLine($"Lastname: {dTOCustomer.Lastname}");
                    Console.WriteLine($"TCKN    : {dTOCustomer.Tckn}");
                    Console.WriteLine($"Password: {dTOCustomer.Password}");
                    Console.WriteLine($"Cell No : {dTOCustomer.CellNo}");
                    Console.WriteLine($"Email   : {dTOCustomer.Email}");
                    Console.WriteLine($"Status  : {dTOCustomer.Status}");
                    Console.WriteLine("================================\n");

                if (dTOCustomer == null)
                {
                    Console.WriteLine("No customer was found that met the given parameters.");
                }
            }
            else
            {
                List<DTOCustomer> dTOCustomers = eCustomer.ReadListCustomer(customer);

                foreach (DTOCustomer dTOCustomer in dTOCustomers)
                {
                    Console.WriteLine($"ID      : {dTOCustomer.CustomerId}");
                    Console.WriteLine($"Name    : {dTOCustomer.Name}");
                    Console.WriteLine($"Lastname: {dTOCustomer.Lastname}");
                    Console.WriteLine($"TCKN    : {dTOCustomer.Tckn}");
                    Console.WriteLine($"Password: {dTOCustomer.Password}");
                    Console.WriteLine($"Cell No : {dTOCustomer.CellNo}");
                    Console.WriteLine($"Email   : {dTOCustomer.Email}");
                    Console.WriteLine($"Status  : {dTOCustomer.Status}");
                    Console.WriteLine("================================\n");
                }

                if (dTOCustomers.Count == 0)
                {
                    Console.WriteLine("No customer was found that met the given parameters.");
                }
            }
        }
    }
}

