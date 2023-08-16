using System;
using FabrieBank.DAL.Common.DTOs;
using System.Data;
using System.Reflection;
using Npgsql;
using NpgsqlTypes;
using FabrieBank.DAL.Common.Enums;

namespace FabrieBank.DAL.Entity
{
    public class ETransactionLog
    {
        private DataAccessLayer dataAccessLayer;
        private NpgsqlConnectionStringBuilder database;

        public ETransactionLog()
        {
            dataAccessLayer = new DataAccessLayer();
            database = dataAccessLayer.CallDB();
        }

        /// <summary>
        /// Read By Id SP caller for TransactionLog
        /// </summary>
        /// <param name="dTOTransactionLog"></param>
        /// <returns></returns>
        public DTOTransactionLog ReadTransactionLog(DTOTransactionLog dTOTransactionLog)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadTransactionLog";

                    string sqlSelectBalance = $"SELECT * FROM {functionName}(@p_log_id)";

                    using (NpgsqlCommand commandSelectBalance = new NpgsqlCommand(sqlSelectBalance, connection))
                    {
                        commandSelectBalance.Parameters.AddWithValue("@p_log_id", dTOTransactionLog.LogId);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandSelectBalance);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        if (dataTable.Rows.Count == 0)
                        {
                            return dTOTransactionLog;
                        }

                        else
                        {
                            dTOTransactionLog = new DTOTransactionLog
                            {
                                LogId = (int)dataTable.Rows[0]["logid"],
                                SourceAccountNumber = (long)dataTable.Rows[0]["sourceaccountnumber"],
                                TargetAccountNumber = (long)dataTable.Rows[0]["targetaccountnumber"],
                                TransactionType = (EnumTransactionType)dataTable.Rows[0]["transactiontype"],
                                TransactionStatus = (EnumTransactionStatus)dataTable.Rows[0]["transactionstatus"],
                                TransferAmount = (decimal)dataTable.Rows[0]["transferamount"],
                                CurrencyRate = (decimal)dataTable.Rows[0]["currencyrate"],
                                TransactionFee = (decimal)dataTable.Rows[0]["transactionfee"],
                                SourceOldBalance = (decimal)dataTable.Rows[0]["sourceoldbalance"],
                                SourceNewBalance = (decimal)dataTable.Rows[0]["sourcenewbalance"],
                                TargetOldBalance = (decimal)dataTable.Rows[0]["targetoldbalance"],
                                TargetNewBalance = (decimal)dataTable.Rows[0]["targetnewbalance"],
                                Timestamp = (DateTime)dataTable.Rows[0]["mytimestamp"],
                                KMV = (decimal)dataTable.Rows[0]["kmv"],
                                SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)dataTable.Rows[0]["sourcecurrencytype"],
                                TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)dataTable.Rows[0]["targetcurrencytype"]
                            };
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
            return dTOTransactionLog;
        }

        /// <summary>
        /// Read List SP caller for Transaction_Log
        /// </summary>
        /// <param name="dTOTransactionLog"></param>
        /// <returns></returns>
        public List<DTOTransactionLog> ReadListTransactionLog(DTOTransactionLog dTOTransactionLog)
        {
            List<DTOTransactionLog> transactionLogs = new List<DTOTransactionLog>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadListTransactionLog";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_source_account_number, @p_target_account_number, @p_transaction_type, @p_transaction_status, @p_transfer_amount_small, @p_transfer_amount_large, @p_currency_rate, @p_start_date, @p_end_date, @p_kmv_small, @p_kmv_large, @p_target_currency_type, @p_source_currency_type)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_source_account_number", NpgsqlDbType.Bigint, dTOTransactionLog.SourceAccountNumber);
                        command.Parameters.AddWithValue("@p_target_account_number", NpgsqlDbType.Bigint, dTOTransactionLog.TargetAccountNumber);
                        command.Parameters.AddWithValue("@p_transaction_type", NpgsqlDbType.Integer, (int)dTOTransactionLog.TransactionType);
                        command.Parameters.AddWithValue("@p_transaction_status", NpgsqlDbType.Integer, (int)dTOTransactionLog.TransactionStatus);
                        command.Parameters.AddWithValue("@p_transfer_amount_small", NpgsqlDbType.Numeric, dTOTransactionLog.TransferAmountSmall);
                        command.Parameters.AddWithValue("@p_transfer_amount_large", NpgsqlDbType.Numeric, dTOTransactionLog.TransferAmountLarge);
                        command.Parameters.AddWithValue("@p_currency_rate", NpgsqlDbType.Numeric, dTOTransactionLog.CurrencyRate);
                        command.Parameters.AddWithValue("@p_start_date", NpgsqlDbType.Date, dTOTransactionLog.StartDate == DateTime.MaxValue ? (object)DBNull.Value : (object)dTOTransactionLog.StartDate.Date);
                        command.Parameters.AddWithValue("@p_end_date", NpgsqlDbType.Date, dTOTransactionLog.EndDate == DateTime.MaxValue ? (object)DBNull.Value : (object)dTOTransactionLog.EndDate.Date);
                        command.Parameters.AddWithValue("@p_kmv_small", NpgsqlDbType.Numeric, dTOTransactionLog.KMVSmall);
                        command.Parameters.AddWithValue("@p_kmv_large", NpgsqlDbType.Numeric, dTOTransactionLog.KMVLarge);
                        command.Parameters.AddWithValue("@p_target_currency_type", NpgsqlDbType.Integer, (int)dTOTransactionLog.TargetCurrencyType);
                        command.Parameters.AddWithValue("@p_source_currency_type", NpgsqlDbType.Integer, (int)dTOTransactionLog.SourceCurrencyType);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);
                        foreach (DataRow item in dataTable.Rows)
                        {
                            DTOTransactionLog transactionLog = new DTOTransactionLog
                            {
                                LogId = (int)item["logid"],
                                SourceAccountNumber = (long)item["sourceaccountnumber"],
                                TargetAccountNumber = (long)item["targetaccountnumber"],
                                TransactionType = (EnumTransactionType)item["transactiontype"],
                                TransactionStatus = (EnumTransactionStatus)item["transactionstatus"],
                                TransferAmount = (decimal)item["transferamount"],
                                CurrencyRate = (decimal)item["currencyrate"],
                                TransactionFee = (decimal)item["transactionfee"],
                                SourceOldBalance = (decimal)item["sourceoldbalance"],
                                SourceNewBalance = (decimal)item["sourcenewbalance"],
                                TargetOldBalance = (decimal)item["targetoldbalance"],
                                TargetNewBalance = (decimal)item["targetnewbalance"],
                                Timestamp = (DateTime)item["mytimestamp"],
                                KMV = (decimal)item["kmv"],
                                SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)dataTable.Rows[0]["sourcecurrencytype"],
                                TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)dataTable.Rows[0]["targetcurrencytype"]
                            };
                            transactionLogs.Add(transactionLog);
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
            return transactionLogs;
        }

        /// <summary>
        /// Insert SP caller for TransactionLog
        /// </summary>
        /// <param name="dTOTransactionLog"></param>
        /// <returns></returns>
        public bool InsertTransactionLog(DTOTransactionLog transactionLog)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string procedureName = "usp_InsertTransactionLog";

                    string sqlQuery = $"CALL {procedureName}(@p_source_account_number, @p_target_account_number, @p_transaction_type, " +
                        $"@p_transaction_status, @p_transfer_amount, @p_currency_rate, @p_transaction_fee, @p_source_old_balance, " +
                        $"@p_source_new_balance, @p_target_old_balance, @p_target_new_balance, @p_timestamp, @p_kmv, @p_target_currency_type, @p_source_currency_type)";



                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_source_account_number", transactionLog.SourceAccountNumber);
                        command.Parameters.AddWithValue("@p_target_account_number", transactionLog.TargetAccountNumber);
                        command.Parameters.AddWithValue("@p_transaction_type", (int)transactionLog.TransactionType);
                        command.Parameters.AddWithValue("@p_transaction_status", (int)transactionLog.TransactionStatus);
                        command.Parameters.AddWithValue("@p_transfer_amount", transactionLog.TransferAmount);
                        command.Parameters.AddWithValue("@p_currency_rate", transactionLog.CurrencyRate);
                        command.Parameters.AddWithValue("@p_transaction_fee", transactionLog.TransactionFee);
                        command.Parameters.AddWithValue("@p_source_old_balance", transactionLog.SourceOldBalance);
                        command.Parameters.AddWithValue("@p_source_new_balance", transactionLog.SourceNewBalance);
                        command.Parameters.AddWithValue("@p_target_old_balance", transactionLog.TargetOldBalance);
                        command.Parameters.AddWithValue("@p_target_new_balance", transactionLog.TargetNewBalance);
                        command.Parameters.AddWithValue("@p_timestamp", DateTime.Now);
                        command.Parameters.AddWithValue("@p_kmv", transactionLog.KMV);
                        command.Parameters.AddWithValue("@p_target_currency_type", (int)transactionLog.TargetCurrencyType);
                        command.Parameters.AddWithValue("@p_source_currency_type", (int)transactionLog.SourceCurrencyType);

                        command.ExecuteNonQuery();
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
            return false;
        }

        /// <summary>
        /// Delete SP caller for TransactionLog
        /// </summary>
        /// <param name="dTOTransactionLog"></param>
        /// <returns></returns>
        public bool DeleteTransactionLog(DTOTransactionLog dTOTransactionLog)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_DeleteTransactionLog";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_log_id)";

                    using (NpgsqlCommand commandDeleteAccount = new NpgsqlCommand(sqlQuery, connection))
                    {
                        commandDeleteAccount.Parameters.AddWithValue("@p_log_id", dTOTransactionLog.LogId);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandDeleteAccount);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            bool success = (bool)dataTable.Rows[0]["func_DeleteTransactionLog"];

                            if (success)
                            {
                                Console.WriteLine("\nLog has been deleted succesfully");
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("\nLog could not be deleted. Please try again.");
                            }
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
            return false;
        }
    }
}

