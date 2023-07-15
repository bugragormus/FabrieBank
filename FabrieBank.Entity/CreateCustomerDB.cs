using System;
using Microsoft.Data.SqlClient;

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

        public void CreateCustomer(string Ad, string Soyad, long Tckn, int Sifre, long TelNo, string Email)
        {
            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                connection.Open();

                // Retrieve the next available MusteriId from the database
                int nextMusteriId = GetNextMusteriId(connection);

                // Enable IDENTITY_INSERT
                string enableIdentityInsertSql = "SET IDENTITY_INSERT dbo.Musteri_Bilgi ON";
                using (SqlCommand enableIdentityInsertCommand = new SqlCommand(enableIdentityInsertSql, connection))
                {
                    enableIdentityInsertCommand.ExecuteNonQuery();
                }

                string sql = "INSERT INTO dbo.Musteri_Bilgi (MusteriId, Ad, Soyad, Tckn, Sifre, TelNo, Email) VALUES (@musteriId, @ad, @soyad, @tckn, @sifre, @telNo, @email)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@musteriId", nextMusteriId);
                    command.Parameters.AddWithValue("@ad", Ad);
                    command.Parameters.AddWithValue("@soyad", Soyad);
                    command.Parameters.AddWithValue("@tckn", Tckn);
                    command.Parameters.AddWithValue("@sifre", Sifre);
                    command.Parameters.AddWithValue("@telNo", TelNo);
                    command.Parameters.AddWithValue("@email", Email);

                    command.ExecuteNonQuery();
                }
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
    }
}