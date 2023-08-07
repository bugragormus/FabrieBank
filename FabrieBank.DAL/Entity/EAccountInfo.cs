using System.Data;
using FabrieBank.DAL.Common.DTOs;
using Npgsql;
using NpgsqlTypes;

namespace FabrieBank.DAL.Entity
{
    public class EAccountInfo
    {
        private DataAccessLayer dataAccessLayer;
        private NpgsqlConnectionStringBuilder database;

        public EAccountInfo()
        {
            dataAccessLayer = new DataAccessLayer();
            database = dataAccessLayer.CallDB();
        }

        /// <summary>
        /// Hesap Tablosundan Tek Satır Döndürür
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        public DTOAccountInfo ReadAccountInfo(DTOAccountInfo accountInfo)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadAccountInfo";

                    string sqlSelectBalance = $"SELECT * FROM {functionName}(@accountNo)";

                    using (NpgsqlCommand commandSelectBalance = new NpgsqlCommand(sqlSelectBalance, connection))
                    {
                        commandSelectBalance.Parameters.AddWithValue("@accountNo", accountInfo.AccountNo);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandSelectBalance);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        if (dataTable.Rows.Count == 0)
                        {
                           // Console.WriteLine("\nHesap bulunamadı.");
                            return accountInfo;
                        }

                        else
                        {
                            accountInfo = new DTOAccountInfo
                            {
                                AccountNo = (long)dataTable.Rows[0]["account_no"],
                                Balance = (decimal)dataTable.Rows[0]["balance"],
                                CustomerId = (int)dataTable.Rows[0]["customer_id"],
                                CurrencyType = (int)dataTable.Rows[0]["currency_type"],
                                AccountName = dataTable.Rows[0]["account_name"].ToString(),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }

            return accountInfo;
        }

        /// <summary>
        /// Hesap Tablosundan Liste Döndürür
        /// </summary>
        /// <param name="dTOAccount"></param>
        /// <returns></returns>
        public List<DTOAccountInfo> ReadListAccountInfo(DTOAccountInfo dTOAccount)
        {
            List<DTOAccountInfo> accountsList = new List<DTOAccountInfo>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadListAccountInfo";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_balance, @p_customer_id, @p_currency_type, @p_account_name)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_balance", NpgsqlDbType.Numeric, dTOAccount.Balance);
                        command.Parameters.AddWithValue("@p_customer_id", NpgsqlDbType.Integer, dTOAccount.CustomerId);
                        command.Parameters.AddWithValue("@p_currency_type", NpgsqlDbType.Integer, dTOAccount.CurrencyType);
                        command.Parameters.AddWithValue("@p_account_name", NpgsqlDbType.Varchar, (object)dTOAccount.AccountName ?? DBNull.Value);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);
                        foreach (DataRow item in dataTable.Rows)
                        {
                            DTOAccountInfo dTOAccountInfo = new DTOAccountInfo
                            {
                                AccountNo = (long)item["account_no"],
                                Balance = (decimal)item["balance"],
                                CustomerId = (int)item["customer_id"],
                                CurrencyType = (int)item["currency_type"],
                                AccountName = item["account_name"].ToString() //!= DBNull.Value ? item["hesap_adi"].ToString() : null
                            };
                            accountsList.Add(dTOAccountInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
            return accountsList;
        }

        /// <summary>
        /// Hesap Tablosuna Veri Gönderir
        /// </summary>
        /// <param name="dTOAccount"></param>
        /// <returns></returns>
        public bool InsertAccountInfo(DTOAccountInfo dTOAccount)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string procedureName = "usp_InsertAccountInfo";

                    string sqlQuery = $"CALL {procedureName}(@p_amount, @p_customer_id, @p_currency_type, @p_account_name)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_amount", dTOAccount.Balance);
                        command.Parameters.AddWithValue("@p_customer_id", dTOAccount.CustomerId);
                        command.Parameters.AddWithValue("@p_currency_type", dTOAccount.CurrencyType);
                        command.Parameters.AddWithValue("@p_account_name", dTOAccount.AccountName);

                        if (command.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
            return false;
        }

        /// <summary>
        /// Hesap Tablosundaki Verileri Yeniler
        /// </summary>
        /// <param name="dTOAccount"></param>
        /// <returns></returns>
        public bool UpdateAccountInfo(DTOAccountInfo dTOAccount)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_UpdateAccountInfo";

                    string sqlQuery = $"CALL {functionName}(@p_account_no, @p_amount, @p_account_name)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_account_no", NpgsqlDbType.Bigint, dTOAccount.AccountNo);
                        command.Parameters.AddWithValue("@p_amount", NpgsqlDbType.Numeric, dTOAccount.Balance);
                        command.Parameters.AddWithValue("@p_account_name", NpgsqlDbType.Varchar, (object)dTOAccount.AccountName ?? DBNull.Value);

                        if (command.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
            return false;
        }

        /// <summary>
        /// Hesap Tablosundan Veri Siler
        /// </summary>
        /// <param name="hesapNo">Müşteri hesap no</param>
        /// <returns></returns>
        public bool DeleteAccountInfo(DTOAccountInfo dTOAccount)
        {
            dTOAccount = ReadAccountInfo(dTOAccount);

            if (dTOAccount != null && dTOAccount.Balance == 0)
            {
                try
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                    {
                        connection.Open();

                        // Delete the account using func_DeleteAccountInfo function
                        string functionName = "func_DeleteAccountInfo";

                        string sqlQuery = $"SELECT * FROM {functionName}(@p_account_no)";

                        using (NpgsqlCommand commandDeleteAccount = new NpgsqlCommand(sqlQuery, connection))
                        {
                            commandDeleteAccount.Parameters.AddWithValue("@p_account_no", dTOAccount.AccountNo);

                            NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandDeleteAccount);
                            DataTable dataTable = new DataTable();

                            npgsqlDataAdapter.Fill(dataTable);

                            // Check the result in the DataTable
                            if (dataTable.Rows.Count > 0)
                            {
                                bool success = (bool)dataTable.Rows[0]["func_deleteaccountinfo"];

                                if (success)
                                {
                                    Console.WriteLine("\nAccount has been deleted succesfully");
                                    return true;
                                }
                                else
                                {
                                    Console.WriteLine("\nThe account could not be deleted. Please try again.");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    EErrorLogger errorLogger = new EErrorLogger();
                    errorLogger.LogAndHandleError(ex);
                }
            }
            else
            {
                Console.WriteLine("\nAccount balance is not 0. Please transfer the balance to another account.");
            }
            return false;
        }
    }
}