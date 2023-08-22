using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank.BLL.Service
{
	public class SCustomer
	{
		public SCustomer()
		{
		}

        public List<DTOCustomer> ReadListCustomer(DTOCustomer customer)
        {
            ECustomer eCustomer = new ECustomer();
            return eCustomer.ReadListCustomer(customer);
        }

        public DTOCustomer ReadCustomer(DTOCustomer Customer)
        {
            ECustomer eCustomer = new ECustomer();
            return eCustomer.ReadCustomer(Customer);
        }

        public bool InsertCustomer(DTOCustomer Customer)
        {
            ECustomer eCustomer = new ECustomer();
            return eCustomer.InsertCustomer(Customer);
        }

        public bool UpdateCustomer(DTOCustomer Customer)
        {
            ECustomer eCustomer = new ECustomer();
            return eCustomer.UpdateCustomer(Customer);
        }

        public bool DeleteCustomer(DTOCustomer Customer)
        {
            ECustomer eCustomer = new ECustomer();
            return eCustomer.DeleteCustomer(Customer);
        }
    }
}