using System;
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
    }
}

