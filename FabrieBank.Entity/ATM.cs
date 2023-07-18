using System;
using FabrieBank.Common;
using Microsoft.Data.SqlClient;

namespace FabrieBank.Entity
{
    public class ATM
    {
        private SqlConnectionStringBuilder database1;
        private Database database;
        private ErrorLogger errorLogger;

        public ATM()
        {
            database = new Database();
            database1 = database.CallDB();
        }

        public void ParaYatirma(long hesapNo, long bakiye)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
                {
                    connection.Open();

                    string sqlSelect = "SELECT Bakiye FROM dbo.Hesap WHERE HesapNo = @hesapNo";
                    string sqlUpdate = "UPDATE dbo.Hesap SET Bakiye = Bakiye + @bakiye WHERE HesapNo = @hesapNo";

                    using (SqlCommand commandSelect = new SqlCommand(sqlSelect, connection))
                    {
                        commandSelect.Parameters.AddWithValue("@hesapNo", hesapNo);

                        long eskiBakiye = (long)commandSelect.ExecuteScalar();
                        long yeniBakiye = eskiBakiye + bakiye;

                        using (SqlCommand commandUpdate = new SqlCommand(sqlUpdate, connection))
                        {
                            commandUpdate.Parameters.AddWithValue("@bakiye", bakiye);
                            commandUpdate.Parameters.AddWithValue("@hesapNo", hesapNo);

                            int rowsAffected = commandUpdate.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("\nPara yatırma işlemi başarılı.");
                                Console.WriteLine($"Eski bakiye: {eskiBakiye}");
                                Console.WriteLine($"Yeni bakiye: {yeniBakiye}");
                            }
                            else
                            {
                                Console.WriteLine("\nPara yatırma işlemi başarısız.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                ErrorLoggerDB logger = new ErrorLoggerDB();
                logger.LogError(ex);

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine("An error occurred while performing ParaYatirma operation. Please try again later.");
            }
        }

        public void ParaCekme(long hesapNo, long bakiye)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
                {
                    connection.Open();

                    string sqlSelect = "SELECT Bakiye FROM dbo.Hesap WHERE HesapNo = @hesapNo";
                    string sqlUpdate = "UPDATE dbo.Hesap SET Bakiye = Bakiye - @bakiye WHERE HesapNo = @hesapNo";

                    using (SqlCommand commandSelect = new SqlCommand(sqlSelect, connection))
                    {
                        commandSelect.Parameters.AddWithValue("@hesapNo", hesapNo);

                        long eskiBakiye = (long)commandSelect.ExecuteScalar();
                        long yeniBakiye = eskiBakiye - bakiye;

                        if (yeniBakiye >= 0)
                        {
                            using (SqlCommand commandUpdate = new SqlCommand(sqlUpdate, connection))
                            {
                                commandUpdate.Parameters.AddWithValue("@bakiye", bakiye);
                                commandUpdate.Parameters.AddWithValue("@hesapNo", hesapNo);

                                int rowsAffected = commandUpdate.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("\nPara çekme işlemi başarılı.");
                                    Console.WriteLine($"Eski bakiye: {eskiBakiye}");
                                    Console.WriteLine($"Yeni bakiye: {yeniBakiye}");
                                }
                                else
                                {
                                    Console.WriteLine("\nPara çekme işlemi başarısız.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n\nYetersiz bakiye. Para çekme işlemi gerçekleştirilemedi.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                ErrorLoggerDB logger = new ErrorLoggerDB();
                logger.LogError(ex);

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine("An error occurred while performing ParaCekme operation. Please try again later.");
            }
        }
    }
}