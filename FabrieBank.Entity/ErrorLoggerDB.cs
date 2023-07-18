using System;
using Microsoft.Data.SqlClient;
using FabrieBank.Common;
using System.Reflection;

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

        public void LogError(Exception ex, string methodName)
        {

            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                connection.Open();

                string sql = "INSERT INTO dbo.ErrorLogs (ErrorDateTime, ErrorMessage, StackTrace, OperationName) VALUES (@errorDateTime, @errorMessage, @stackTrace, @operationName)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@errorDateTime", DateTime.Now);
                    command.Parameters.AddWithValue("@errorMessage", ex.Message);
                    command.Parameters.AddWithValue("@stackTrace", ex.StackTrace);
                    command.Parameters.AddWithValue("@operationName", methodName);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
