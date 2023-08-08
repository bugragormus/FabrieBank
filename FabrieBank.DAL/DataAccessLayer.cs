using FabrieBank.DAL.Common.Enums;
using Npgsql;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;
using System.Text;
using System.Security.Cryptography;

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
        /// DB Bağlanma
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
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
                return new NpgsqlConnectionStringBuilder();
            }
        }

        /// <summary>
        /// EErrorLogger.cs
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="methodName"></param>
        public void LogError(Exception ex, string methodName)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
            {
                connection.Open();

                string procedureName = "usp_InsertErrorLog";

                string sqlQuery = $"CALL {procedureName}(@errorDateTime, @errorMessage, @stackTrace, @operationName)";

                using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("errorDateTime", DateTime.Now);
                    command.Parameters.AddWithValue("errorMessage", ex.Message);
                    command.Parameters.AddWithValue("stackTrace", ex.StackTrace);
                    command.Parameters.AddWithValue("operationName", methodName);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// TransactionLogDB.cs
        /// </summary>
        /// <param name="transactionLog"></param>
        public void LogTransaction(DTOTransactionLog transactionLog)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string procedureName = "usp_InsertTransactionLog";

                    string sqlQuery = $"CALL {procedureName}(@p_accountNumber, @p_targetAccountNumber, @p_transactionType, @p_transactionStatus, @p_amount, @p_oldBalance, @p_newBalance, @p_transactionFee, @p_timestamp)";



                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_accountNumber", transactionLog.AccountNumber);
                        command.Parameters.AddWithValue("@p_targetAccountNumber", transactionLog.TargetAccountNumber);
                        command.Parameters.AddWithValue("@p_transactionType", (int)transactionLog.TransactionType);
                        command.Parameters.AddWithValue("@p_transactionStatus", (int)transactionLog.TransactionStatus);
                        command.Parameters.AddWithValue("@p_amount", transactionLog.Amount);
                        command.Parameters.AddWithValue("@p_oldBalance", transactionLog.OldBalance);
                        command.Parameters.AddWithValue("@p_newBalance", transactionLog.NewBalance);
                        command.Parameters.AddWithValue("@p_transactionFee", transactionLog.TransactionFee);
                        command.Parameters.AddWithValue("@p_timestamp", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

        /// <summary>
        /// TransactionFeeDB.cs
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
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }

            return 0.00m;
        }

        /// <summary>
        /// It hashes passwords
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static string ComputeSha256Hash(string rawData)
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