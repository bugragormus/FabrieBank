using System;
using Microsoft.Data.SqlClient;
using FabrieBank.Common;
using FabrieBank.DAL;
using Npgsql;

namespace FabrieBank.Entity
{
    public class CreateCustomerDB
    {
        private DataAccessLayer dataAccessLayer;

        public CreateCustomerDB()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public void CreateCustomer(DTOCustomer customer)
        {
            dataAccessLayer.CreateCustomer(customer);
        }

        public int GetNextMusteriId(NpgsqlConnection connection)
        {
            return dataAccessLayer.GetNextMusteriId(connection);
        }

        public bool IsCustomerExists(NpgsqlConnection connection, long tckn)
        {
            return dataAccessLayer.IsCustomerExists(connection, tckn);
        }
    }
}