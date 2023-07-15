//using System;
//using Microsoft.Data.SqlClient;
//using FabrieBank.Common;

//namespace FabrieBank.Entity
//{
//    public class TransactionDB
//    {
//        private SqlConnectionStringBuilder database1;
//        private Database database;

//        public TransactionDB()
//        {
//            database = new Database();
//            database1 = database.CallDB();
//        }

//        public void Deposit(long hesapNo, long amount)
//        {
//            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
//            {
//                connection.Open();

//                string sql = "UPDATE dbo.Hesap SET Bakiye = Bakiye + @amount WHERE HesapNo = @hesapNo";

//                using (SqlCommand command = new SqlCommand(sql, connection))
//                {
//                    command.Parameters.AddWithValue("@amount", amount);
//                    command.Parameters.AddWithValue("@hesapNo", hesapNo);

//                    int rowsAffected = command.ExecuteNonQuery();

//                    if (rowsAffected == 0)
//                    {
//                        Console.WriteLine("Hesap bulunamadı. Para yatırma işlemi gerçekleştirilemedi.");
//                    }
//                }
//            }
//        }

//        public void Withdraw(long hesapNo, long amount)
//        {
//            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
//            {
//                connection.Open();

//                string sql = "UPDATE dbo.Hesap SET Bakiye = Bakiye - @amount WHERE HesapNo = @hesapNo AND Bakiye >= @amount";

//                using (SqlCommand command = new SqlCommand(sql, connection))
//                {
//                    command.Parameters.AddWithValue("@amount", amount);
//                    command.Parameters.AddWithValue("@hesapNo", hesapNo);

//                    int rowsAffected = command.ExecuteNonQuery();

//                    if (rowsAffected == 0)
//                    {
//                        Console.WriteLine("Hesap bulunamadı veya yetersiz bakiye. Para çekme işlemi gerçekleştirilemedi.");
//                    }
//                }
//            }
//        }
//    }
//}
