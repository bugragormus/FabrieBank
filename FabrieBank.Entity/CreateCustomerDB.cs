using System;
using Microsoft.Data.SqlClient;
using FabrieBank.Common;

namespace FabrieBank.Entity
{
    public class CreateCustomerDB
    {
        private SqlConnectionStringBuilder database1;
        private Database database;

        public CreateCustomerDB()
        {
            database = new Database();
            database1 = database.CallDB();
        }

        public void CreateCustomer(DTOCustomer customer)
        {
            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                connection.Open();

                if (IsCustomerExists(connection, customer.Tckn))
                {
                    Console.WriteLine("\nCustomer already exists.");
                    return;
                }

                int nextMusteriId = GetNextMusteriId(connection);

                string enableIdentityInsertSql = "SET IDENTITY_INSERT dbo.Musteri_Bilgi ON";
                using (SqlCommand enableIdentityInsertCommand = new SqlCommand(enableIdentityInsertSql, connection))
                {
                    enableIdentityInsertCommand.ExecuteNonQuery();
                }

                string sql = "INSERT INTO dbo.Musteri_Bilgi (MusteriId, Ad, Soyad, Tckn, Sifre, TelNo, Email) VALUES (@musteriId, @ad, @soyad, @tckn, @sifre, @telNo, @email)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@musteriId", nextMusteriId);
                    command.Parameters.AddWithValue("@ad", customer.Ad);
                    command.Parameters.AddWithValue("@soyad", customer.Soyad);
                    command.Parameters.AddWithValue("@tckn", customer.Tckn);
                    command.Parameters.AddWithValue("@sifre", customer.Sifre);
                    command.Parameters.AddWithValue("@telNo", customer.TelNo);
                    command.Parameters.AddWithValue("@email", customer.Email);

                    command.ExecuteNonQuery();
                }
                Console.Clear();
            }
        }

        private int GetNextMusteriId(SqlConnection connection)
        {
            string sql = "SELECT ISNULL(MAX(MusteriId), 0) + 1 FROM dbo.Musteri_Bilgi";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                int nextMusteriId = (int)command.ExecuteScalar();
                return nextMusteriId;
            }
        }

        private bool IsCustomerExists(SqlConnection connection, long tckn)
        {
            string sql = "SELECT COUNT(*) FROM dbo.Musteri_Bilgi WHERE Tckn = @tckn";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@tckn", tckn);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }
    }
}