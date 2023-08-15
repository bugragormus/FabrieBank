using FabrieBank.DAL.Common.Enums;
using Npgsql;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;

namespace FabrieBank.DAL
{
    public class DataAccessLayer
    {
        private NpgsqlConnectionStringBuilder database;

        public DataAccessLayer()
        {
            database = CallDB();
        }

        /// <summary>
        /// DB connection
        /// </summary>
        /// <returns></returns>
        public NpgsqlConnectionStringBuilder CallDB()
        {
            try
            {
                NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

                builder.Host = "localhost";
                builder.Username = "postgres";
                builder.Password = "43324332";
                builder.Database = "FabrieBank";

                return builder;
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return new NpgsqlConnectionStringBuilder();
            }
        }

        /// <summary>
        /// Calls Insert SP for error logging
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="methodName"></param>
        //public void LogError(Exception ex, string methodName)
        //{
        //    using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
        //    {
        //        connection.Open();

        //        string procedureName = "usp_InsertErrorLog";

        //        string sqlQuery = $"CALL {procedureName}(@errorDateTime, @errorMessage, @stackTrace, @operationName)";

        //        using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
        //        {
        //            command.Parameters.AddWithValue("errorDateTime", DateTime.Now);
        //            command.Parameters.AddWithValue("errorMessage", ex.Message);
        //            command.Parameters.AddWithValue("stackTrace", ex.StackTrace);
        //            command.Parameters.AddWithValue("operationName", methodName);

        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        /// <summary>
        /// Gets transaction fees from DB
        /// </summary>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public decimal GetTransactionFee(EnumTransactionFeeType transactionType)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_readtransactionfee";

                    string sql = $"SELECT * FROM {functionName}(@transactionType)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        // Convert the EnumTransactionFeeType to its integer representation
                        int transactionTypeValue = (int)transactionType;
                        command.Parameters.AddWithValue("@transactionType", transactionTypeValue);

                        object result = command.ExecuteScalar();

                        if (result != null && decimal.TryParse(result.ToString(), out decimal feeAmount))
                        {
                            return feeAmount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return 0.00m;
        }

        /// <summary>
        /// Password hashing algorithm
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static string ComputeHash(string rawData)
        {
            string salt = "FabrieBankPasswordSafety";

            byte[] combinedBytes = Encoding.UTF8.GetBytes(rawData + salt);

            using (SHA256 sha256Hash = SHA256.Create())
            {

                byte[] hashBytes = sha256Hash.ComputeHash(combinedBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}