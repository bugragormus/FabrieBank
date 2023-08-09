using System.Data;
using Npgsql;
using NpgsqlTypes;
using FabrieBank.DAL.Common.DTOs;

namespace FabrieBank.DAL.Entity
{
    public class ECurrency
    {
        private DataAccessLayer dataAccessLayer;
        private NpgsqlConnectionStringBuilder database;

        public ECurrency()
        {
            dataAccessLayer = new DataAccessLayer();
            database = dataAccessLayer.CallDB();
        }

        /// <summary>
        /// Read By Id SP caller for Currencies
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public DTOCurrency ReadCurrency(DTOCurrency currency)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadCurrency";

                    string vsql = $"SELECT * FROM {functionName}(@p_id)";

                    using (NpgsqlCommand command = new NpgsqlCommand(vsql, connection))
                    {
                        command.Parameters.AddWithValue("@p_id", currency.Id);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        DTOCurrency dTOCurrency = new DTOCurrency
                        {
                            Id = (int)dataTable.Rows[0]["id"],
                            CurrencyType = dataTable.Rows[0]["currency_type"].ToString(),
                        };
                        if (dataTable.Rows.Count == 0)
                        {
                            //Console.WriteLine("\nHesap bulunamadı.");
                            return dTOCurrency;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
            return currency;
        }

        /// <summary>
        /// Read List SP caller for Currencies
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public List<DTOCurrency> ReadListCurrency(DTOCurrency currency)
        {
            List<DTOCurrency> currenciesList = new List<DTOCurrency>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadListCurrency";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_type)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_type", NpgsqlDbType.Varchar, (object)currency.CurrencyType ?? DBNull.Value);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);
                        foreach (DataRow item in dataTable.Rows)
                        {
                            DTOCurrency dTOCurrency = new DTOCurrency
                            {
                                Id = (int)item["id"],
                                CurrencyType = item["currency_type"].ToString(),
                            };
                            currenciesList.Add(dTOCurrency);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
            return currenciesList;
        }

        /// <summary>
        /// Insert SP caller for Currencies
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public bool InsertCurrency(DTOCurrency currency)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_InsertCurrency";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_id, @p_type)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_id", NpgsqlDbType.Integer, currency.Id);
                        command.Parameters.AddWithValue("@p_type", NpgsqlDbType.Varchar, currency.CurrencyType);

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
        /// Update SP caller for Currencies
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public bool UpdateCurrency(DTOCurrency currency)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_UpdateCurrency";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_id, @p_type)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_id", NpgsqlDbType.Integer, currency.Id);
                        command.Parameters.AddWithValue("@p_type", NpgsqlDbType.Varchar, currency.CurrencyType);

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
        /// Delete SP caller for Currencies
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public bool DeleteCurrency(DTOCurrency currency)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    // Delete the account using func_DeleteAccountInfo function
                    string functionName = "func_DeleteCurrency";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_id)";

                    using (NpgsqlCommand commandDeleteHesap = new NpgsqlCommand(sqlQuery, connection))
                    {
                        commandDeleteHesap.Parameters.AddWithValue("@p_id", currency.Id);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandDeleteHesap);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        // Check the result in the DataTable
                        if (dataTable.Rows.Count > 0)
                        {
                            bool success = (bool)dataTable.Rows[0]["func_DeleteCurrency"];

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
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
            return false;
        }
    }
}