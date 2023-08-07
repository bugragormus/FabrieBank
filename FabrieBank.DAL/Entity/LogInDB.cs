using FabrieBank.DAL.Common.DTOs;

namespace FabrieBank.DAL.Entity
{
    public class LogInDB
    {
        private ECustomer eCustomer;

        public LogInDB()
        {
            eCustomer = new ECustomer();
        }

        public DTOCustomer LogIn(long tckn, int password)
        {
            return eCustomer.LogIn(tckn, password);
        }

        public bool UpdatePersonelInfo(DTOCustomer customer)
        {
            return eCustomer.UpdatePersonelInfo(customer);
        }

        public bool ForgotPassword(DTOCustomer customer, string email)
        {
            int temporaryPassword = GenerateTemporaryPassword();
            return eCustomer.ForgotPassword(customer, email, temporaryPassword);
        }

        private int GenerateTemporaryPassword()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }

        public bool ChangePassword(DTOCustomer customer)
        {
            return eCustomer.ChangePassword(customer);
        }
    }
}