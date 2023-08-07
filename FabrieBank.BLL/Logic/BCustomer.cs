using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank.BLL.Logic
{
	public class BCustomer
	{
        private ECustomer eCustomer;

		public BCustomer()
		{
            eCustomer = new ECustomer();
		}

        /// <summary>
        /// Müşteri Oluşturur
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
        /// Giriş işlemi yapar
        /// </summary>
        /// <param name="tckn"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public DTOCustomer LogIn(long tckn, int password)
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
        /// İletişim bilgileri güncelleme
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool UpdatePersonelInfo(DTOCustomer customer)
        {
            eCustomer.UpdateCustomer(customer);
            return true;
        }

        /// <summary>
        /// Şifre değiştirir
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool ChangePassword(DTOCustomer customer)
        {
            eCustomer.UpdateCustomer(customer);
            return true;
        }

        /// <summary>
        /// Şifremi unuttum işlemleri
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="email"></param>
        /// <param name="temporaryPassword"></param>
        /// <returns></returns>
        public bool ForgotPassword(DTOCustomer customer, string email)
        {
            int temporaryPassword = GenerateTemporaryPassword();
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
                    Password = temporaryPassword,
                    CellNo = customer.CellNo,
                    Email = customer.Email
                };

                eCustomer.UpdateCustomer(dTOCustomer);
                Console.WriteLine($"\nYour temporary password has been successfully created. Your password: {temporaryPassword}");
                return true;
            }
        }

        /// <summary>
        /// Generates temp password
        /// </summary>
        /// <returns></returns>
        private int GenerateTemporaryPassword()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }
    }
}

