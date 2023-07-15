using System;
using Microsoft.Data.SqlClient;

namespace FabrieBank.Entity
{
    public class BringAccount
    {
        private SqlConnectionStringBuilder database1;
        private Database database;

        public BringAccount()
        {
            database = new Database();
            database1 = database.CallDB();
        }

        public string BringAllAccounts()
        {
            string result = "";

            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                Console.WriteLine("\nSisteme Kayıtlı Müşteriler:");
                Console.WriteLine("=========================================\n");

                connection.Open();

                string sql = "SELECT * FROM Musteri_Bilgi;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("{0} {1} {2} {3} {4} {5} {6}", reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt64(3), reader.GetInt32(4), reader.GetInt64(5), reader.GetString(6));
                        }
                    }
                }
            }
            return result;
        }
    }
}
