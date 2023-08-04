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
        /// Dovizler Tablosundan Tek Veri Döndürür
        /// </summary>
        /// <param name="customer"></param>
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
                            DovizCins = dataTable.Rows[0]["cins"].ToString(),
                        };
                        if (dataTable.Rows.Count == 0)
                        {
                            Console.WriteLine("\nHesap bulunamadı.");
                            return dTOCurrency;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }
            return currency;
        }

        /// <summary>
        /// Dovizler Tablosundan Liste Döndürür
        /// </summary>
        /// <param name="dTOAccount"></param>
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

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_cins)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_cins", NpgsqlDbType.Varchar, (object)currency.DovizCins ?? DBNull.Value);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);
                        foreach (DataRow item in dataTable.Rows)
                        {
                            DTOCurrency dTOCurrency = new DTOCurrency
                            {
                                Id = (int)item["id"],
                                DovizCins = item["cins"].ToString(),
                            };
                            currenciesList.Add(dTOCurrency);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }
            return currenciesList;
        }

        /// <summary>
        /// Dovizler Tablosuna Veri Gönderir
        /// </summary>
        /// <param name="dTOAccount"></param>
        /// <returns></returns>
        public bool InsertCurrency(DTOCurrency currency)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_InsertCurrency";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_id, @p_cins)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_id", NpgsqlDbType.Integer, currency.Id);
                        command.Parameters.AddWithValue("@p_cins", NpgsqlDbType.Varchar, currency.DovizCins);

                        if (command.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }
            return false;
        }

        /// <summary>
        /// Dovizler Tablosundaki Verileri Yeniler
        /// </summary>
        /// <param name="dTOAccount"></param>
        /// <returns></returns>
        public bool UpdateCurrency(DTOCurrency currency)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_UpdateCurrency";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_id, @p_cins)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_id", NpgsqlDbType.Integer, currency.Id);
                        command.Parameters.AddWithValue("@p_cins", NpgsqlDbType.Varchar, currency.DovizCins);

                        if (command.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }
            return false;
        }

        /// <summary>
        /// Dovizler Tablosundan Veri Siler
        /// </summary>
        /// <param name="hesapNo">Müşteri hesap no</param>
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
                                Console.WriteLine("\nMüşteri başarıyla silindi.");
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("\nMüşteri silinemedi. Lütfen tekrar deneyin.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }
            return false;
        }
    }
}