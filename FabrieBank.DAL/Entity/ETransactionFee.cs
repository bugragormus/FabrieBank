using System;
using FabrieBank.DAL.Common.DTOs;
using System.Data;
using System.Reflection;
using Npgsql;
using NpgsqlTypes;
using FabrieBank.DAL.Common.Enums;

namespace FabrieBank.DAL.Entity
{
	public class ETransactionFee
	{
        private DataAccessLayer dataAccessLayer;
        private NpgsqlConnectionStringBuilder database;

        public ETransactionFee()
		{
            dataAccessLayer = new DataAccessLayer();
            database = dataAccessLayer.CallDB();
        }

        /// <summary>
        /// Read By Id SP caller for TransactionFee
        /// </summary>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public decimal ReadTransactionFee(EnumTransactionFeeType transactionType)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_readtransactionfee";

                    string sql = $"SELECT * FROM {functionName}(@p_transactionType)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        int transactionTypeValue = (int)transactionType;
                        command.Parameters.AddWithValue("@p_transactionType", transactionTypeValue);

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
        /// Read List SP caller for TransactionFee
        /// </summary>
        /// <param name="dTOTransactionFee"></param>
        /// <returns></returns>
        public List<DTOTransactionFee> ReadListTransactionFee(DTOTransactionFee dTOTransactionFee)
        {
            List<DTOTransactionFee> transactionFees = new List<DTOTransactionFee>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadListTransactionFee";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_fee_amount_is_small, @p_fee_amount_is_big)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_fee_amount_is_small", NpgsqlDbType.Numeric, dTOTransactionFee.AmountIsSmall);
                        command.Parameters.AddWithValue("@p_fee_amount_is_big", NpgsqlDbType.Numeric, dTOTransactionFee.AmountIsBig);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);
                        foreach (DataRow item in dataTable.Rows)
                        {
                            DTOTransactionFee transactionFee = new DTOTransactionFee
                            {
                                FeeType = (EnumTransactionFeeType)item["transactiontype"],
                                Amount = (decimal)item["feeamount"]
                            };
                            transactionFees.Add(transactionFee);
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
            return transactionFees;
        }

        /// <summary>
        /// Update SP caller for TransactionFee
        /// </summary>
        /// <param name="dTOTransactionFee"></param>
        /// <returns></returns>
        public bool UpdateTransactionFee(DTOTransactionFee dTOTransactionFee)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_UpdateTransactionFee";

                    string sqlQuery = $"CALL {functionName}(@p_fee_amount)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_fee_amount", NpgsqlDbType.Numeric, dTOTransactionFee.Amount);

                        if (command.ExecuteNonQuery() > 0)
                        {
                            return true;
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

        /// <summary>
        /// Insert SP caller for TransactionFee
        /// </summary>
        /// <param name="dTOTransactionFee"></param>
        /// <returns></returns>
        public bool InsertTransactionFee(DTOTransactionFee dTOTransactionFee)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_InsertTransactionFee";

                    string sqlQuery = $"CALL {functionName}(@p_fee_amount)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_fee_amount", NpgsqlDbType.Numeric, dTOTransactionFee.Amount);

                        if (command.ExecuteNonQuery() > 0)
                        {
                            return true;
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

        /// <summary>
        /// Delete SP caller for TransactionFee
        /// </summary>
        /// <param name="dTOTransactionFee"></param>
        /// <returns></returns>
        public bool DeleteTransactionFee(DTOTransactionFee dTOTransactionFee)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    // Delete the account using func_DeleteAccountInfo function
                    string functionName = "func_DeleteTransactionFee";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_transaction_type)";

                    using (NpgsqlCommand commandDeleteHesap = new NpgsqlCommand(sqlQuery, connection))
                    {
                        commandDeleteHesap.Parameters.AddWithValue("@p_transaction_type", dTOTransactionFee.FeeType);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandDeleteHesap);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        // Check the result in the DataTable
                        if (dataTable.Rows.Count > 0)
                        {
                            bool success = (bool)dataTable.Rows[0]["func_DeleteTransactionFee"];

                            if (success)
                            {
                                Console.WriteLine("\nThe customer has been successfully deleted.");
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("\nThe customer could not be deleted. Please try again.");
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

