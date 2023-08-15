using FabrieBank.Admin.DAL.Common.DTO;
using FabrieBank.Admin.DAL.Entity;
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
    }
}

