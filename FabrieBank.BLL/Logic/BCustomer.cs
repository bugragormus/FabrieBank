using FabrieBank.DAL;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank.BLL.Logic
{
    public class BCustomer
    {
        private ECustomer eCustomer;
        private EAccountInfo eAccountInfo;

        public BCustomer()
        {
            eCustomer = new ECustomer();
            eAccountInfo = new EAccountInfo();
        }

        /// <summary>
        /// Customer creation process
        /// </summary>
        /// <param name="customer"></param>
        public void CreateCustomer(DTOCustomer customer)
        {

            DTOCustomer existingCustomer = eCustomer.ReadCustomer(customer);

            if (existingCustomer != null)
            {
                Console.WriteLine("\nA customer with the same TCKN already exists.\n");
            }
            else
            {
                _ = eCustomer.InsertCustomer(customer);
                Console.WriteLine("\nA customer created succesfully.\n");
            }
        }

        /// <summary>
        /// Login process
        /// </summary>
        /// <param name="tckn"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public DTOCustomer LogIn(long tckn, string password)
        {
            DTOCustomer customer = eCustomer.ReadCustomer(new DTOCustomer { Tckn = tckn });

            if (customer != null && customer.Password == password)
            {
                return customer;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Personel info updating process
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool UpdatePersonelInfo(DTOCustomer customer)
        {
            eCustomer.UpdateCustomer(customer);
            return true;
        }

        /// <summary>
        /// Password updating process
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool ChangePassword(DTOCustomer customer)
        {
            eCustomer.UpdateCustomer(customer);
            return true;
        }

        /// <summary>
        /// Forgotten password process
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool ForgotPassword(DTOCustomer customer, string email)
        {
            int temporaryPassword = GenerateTemporaryPassword();
            string hashedPassword = DataAccessLayer.ComputeHash(temporaryPassword.ToString());
            customer = eCustomer.ReadCustomer(customer);

            if (customer.Email != email)
            {
                Console.WriteLine("The information you have provided does not match. Please try again.");
                return false;
            }
            else
            {
                DTOCustomer dTOCustomer = new DTOCustomer()
                {
                    CustomerId = customer.CustomerId,
                    Name = customer.Name,
                    Lastname = customer.Lastname,
                    Password = hashedPassword,
                    CellNo = customer.CellNo,
                    Email = customer.Email,
                    Status = customer.Status
                };

                eCustomer.UpdateCustomer(dTOCustomer);
                Console.WriteLine($"\nYour temporary password has been successfully created. Your password: {temporaryPassword}");
                return true;
            }
        }

        /// <summary>
        /// Customer deactivation process
        /// </summary>
        /// <param name="customer"></param>
        public void Deactivation(DTOCustomer customer)
        {
            DTOAccountInfo dTOAccountInfo = new DTOAccountInfo() { CustomerId = customer.CustomerId };
            List<DTOAccountInfo> existingAccounts = eAccountInfo.ReadListAccountInfo(dTOAccountInfo);
            if (existingAccounts.Count != 0)
            {
                Console.WriteLine("\nYou have active accounts. Please delete them before deactivation.\n");
            }
            else
            {
                _ = eCustomer.UpdateCustomer(customer);
                Console.WriteLine("\nStatus has been updated to Inactive.\n");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Generates temporary password for forgetting process
        /// </summary>
        /// <returns></returns>
        private int GenerateTemporaryPassword()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }
    }
}

