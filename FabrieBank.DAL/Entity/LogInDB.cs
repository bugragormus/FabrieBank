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

        public bool UpdatePersonelInfo(int musteriId, long telNo, string email)
        {
            return dataAccessLayer.UpdatePersonelInfo(musteriId, telNo, email);
        }

        public bool IsCredentialsValid(long tckn, int sifre)
        {
            return dataAccessLayer.IsCredentialsValid(tckn, sifre);
        }

        public bool ForgotPassword(long tckn, string email)
        {
            int temporaryPassword = GenerateTemporaryPassword();
            return dataAccessLayer.ForgotPassword(tckn, email, temporaryPassword);
        }

        private int GenerateTemporaryPassword()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }

        public bool ChangePassword(int musteriId, int newPassword)
        {
            return dataAccessLayer.ChangePassword(musteriId, newPassword);
        }
    }
}
