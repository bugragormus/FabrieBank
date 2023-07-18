using System;
using Microsoft.Data.SqlClient;
using FabrieBank.Common;

namespace FabrieBank.Entity
{
    public class ErrorLoggerDB : IErrorLogger
    {
        private SqlConnectionStringBuilder database1;
        private Database database;

        public ErrorLoggerDB()
        {
            database = new Database();
            database1 = database.CallDB();
        }

        public void LogError(Exception ex)
        {
            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                connection.Open();

                string sql = "INSERT INTO dbo.ErrorLogs (ErrorDateTime, ErrorMessage, StackTrace) VALUES (@logTime, @errorMessage, @stackTrace)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@logTime", DateTime.Now);
                    command.Parameters.AddWithValue("@errorMessage", ex.Message);
                    command.Parameters.AddWithValue("@stackTrace", ex.StackTrace);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
