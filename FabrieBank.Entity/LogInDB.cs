using System;
using Microsoft.Data.SqlClient;
using FabrieBank.Common;

namespace FabrieBank.Entity
{
    public class LogInDB
    {
        private SqlConnectionStringBuilder database1;
        private Database database;

        public LogInDB()
        {
            database = new Database();
            database1 = database.CallDB();
        }

        public DTOCustomer LogIn(long tckn, int sifre)
        {
            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM dbo.Musteri_Bilgi WHERE Tckn = @tckn AND Sifre = @sifre";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tckn", tckn);
                    command.Parameters.AddWithValue("@sifre", sifre);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DTOCustomer customer = new DTOCustomer()
                            {
                                MusteriId = reader.GetInt32(0),
                                Ad = reader.GetString(1),
                                Soyad = reader.GetString(2),
                                Tckn = reader.GetInt64(3),
                                Sifre = reader.GetInt32(4),
                                TelNo = reader.GetInt64(5),
                                Email = reader.GetString(6)
                            };

                            return customer;
                        }
                    }
                }
            }

            return new DTOCustomer();
        }
    }
}
