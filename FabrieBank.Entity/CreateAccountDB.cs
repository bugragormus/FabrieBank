using System;
using Microsoft.Data.SqlClient;

namespace FabrieBank.Entity
{
    public class CreateAccountDB
    {
        private SqlConnectionStringBuilder database1;
        private Database database;

        public CreateAccountDB()
        {
            database = new Database();
            database1 = database.CallDB();
        }

        public void CreateAccount(int musteriId, int dovizCinsi, string hesapAdi)
        {
            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                connection.Open();

                // Get account number and increment 
                string hesapNumarasi = GetAndIncrementHesapNumarasi(dovizCinsi, connection);

                // Add to Hesap table
                string sql = "INSERT INTO dbo.Hesap (HesapNo, Bakiye, MusteriId, DovizCins, HesapAdi) VALUES (@hesapNo, 0, @musteriId, @dovizCinsi, @hesapAdi)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@hesapNo", hesapNumarasi);
                    command.Parameters.AddWithValue("@musteriId", musteriId);
                    command.Parameters.AddWithValue("@dovizCinsi", dovizCinsi);
                    command.Parameters.AddWithValue("@hesapAdi", hesapAdi);

                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"\n'{hesapNumarasi}' Numaralı yeni hesap oluşturuldu.\n");
            }
        }

        private string GetAndIncrementHesapNumarasi(int dovizCinsi, SqlConnection connection)
        {
            string sqlSelect = "SELECT HesapNumarasi FROM dbo.Hesap_No WHERE DovizCinsi = @dovizCinsi";
            string sqlUpdate = "UPDATE dbo.Hesap_No SET HesapNumarasi = HesapNumarasi + 1 WHERE DovizCinsi = @dovizCinsi";

            string hesapNumarasi = string.Empty;

            using (SqlCommand commandSelect = new SqlCommand(sqlSelect, connection))
            {
                commandSelect.Parameters.AddWithValue("@dovizCinsi", dovizCinsi);

                using (SqlDataReader reader = commandSelect.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hesapNumarasi = reader.GetInt64(0).ToString();
                    }
                }
            }

            using (SqlCommand commandUpdate = new SqlCommand(sqlUpdate, connection))
            {
                commandUpdate.Parameters.AddWithValue("@dovizCinsi", dovizCinsi);
                commandUpdate.ExecuteNonQuery();
            }

            return hesapNumarasi;
        }
    }
}