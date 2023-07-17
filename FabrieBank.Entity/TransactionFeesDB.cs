using System;
using Microsoft.Data.SqlClient;

namespace FabrieBank.Entity
{
    public class TransactionFeesDB
    {
        private SqlConnectionStringBuilder database1;
        private Database database;

        public TransactionFeesDB()
        {
            database = new Database();
            database1 = database.CallDB();
        }

        public bool CheckSufficientBalance(long kaynakHesapNo, long transferMiktar)
        {
            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                connection.Open();

                string sqlSelect = "SELECT Bakiye FROM dbo.Hesap WHERE HesapNo = @hesapNo";

                using (SqlCommand commandSelect = new SqlCommand(sqlSelect, connection))
                {
                    commandSelect.Parameters.AddWithValue("@hesapNo", kaynakHesapNo);

                    long bakiye = (long)commandSelect.ExecuteScalar();
                    long minimumBakiye = 5; // Transfer ücreti için minimum bakiye

                    return bakiye >= transferMiktar + minimumBakiye;
                }
            }
        }
    }
}
