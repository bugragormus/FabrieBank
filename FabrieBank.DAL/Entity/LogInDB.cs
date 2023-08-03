using FabrieBank.DAL;
using FabrieBank.DAL.Common.DTOs;

namespace FabrieBank.DAL.Entity
{
    public class LogInDB
    {
        private DataAccessLayer dataAccessLayer;
        private ECustomer eCustomer;

        public LogInDB()
        {
            dataAccessLayer = new DataAccessLayer();
            eCustomer = new ECustomer();
        }

        public DTOCustomer LogIn(long tckn, int sifre)
        {
            return eCustomer.LogIn(tckn, sifre);
        }

        public bool UpdatePersonelInfo(DTOCustomer customer)
        {
            return eCustomer.UpdatePersonelInfo(customer);
        }

        public bool IsCredentialsValid(long tckn, int sifre)
        {
            return dataAccessLayer.IsCredentialsValid(tckn, sifre);
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
