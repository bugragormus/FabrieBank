using System;
using FabrieBank.DAL.Common.DTOs;
using System.Data;
using System.Reflection;
using Npgsql;
using NpgsqlTypes;
using System.Security.Principal;

namespace FabrieBank.DAL.Entity
{
    public class EErrorLog
    {
        private DataAccessLayer dataAccessLayer;
        private NpgsqlConnectionStringBuilder database;

        public EErrorLog()
        {
            dataAccessLayer = new DataAccessLayer();
            database = dataAccessLayer.CallDB();
        }

        /// <summary>
        /// Read By Id SP caller for ReadErrorLog
        /// </summary>
        /// <param name="dTOErrorLog"></param>
        /// <returns></returns>
        public DTOErrorLog ReadErrorLog(DTOErrorLog dTOErrorLog)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadErrorLog";

                    string sqlSelectBalance = $"SELECT * FROM {functionName}(@p_id)";

                    using (NpgsqlCommand commandSelectBalance = new NpgsqlCommand(sqlSelectBalance, connection))
                    {
                        commandSelectBalance.Parameters.AddWithValue("@p_id", dTOErrorLog.ErrorId);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandSelectBalance);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        if (dataTable.Rows.Count == 0)
                        {
                            return dTOErrorLog;
                        }

                        else
                        {
                            dTOErrorLog = new DTOErrorLog
                            {
                                ErrorId = (int)dataTable.Rows[0]["id"],
                                ErrorDateTime = (DateTime)dataTable.Rows[0]["errordatetime"],
                                ErrorMessage = dataTable.Rows[0]["errormessage"].ToString(),
                                StackTrace = dataTable.Rows[0]["stacktrace"].ToString(),
                                OperationName = dataTable.Rows[0]["operationname"].ToString(),
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
            return dTOErrorLog;
        }

        /// <summary>
        /// Read List SP caller for ReadListErrorLog
        /// </summary>
        /// <param name="dTOErrorLog"></param>
        /// <returns></returns>
        public List<DTOErrorLog> ReadListErrorLog(DTOErrorLog dTOErrorLog)
        {
            List<DTOErrorLog> errorLogs = new List<DTOErrorLog>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    DateTime startDate = dTOErrorLog.StartDate.Date;
                    DateTime endDate = dTOErrorLog.EndDate.Date;
                    string functionName = "func_ReadListErrorLog";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_start_date, @p_end_date, @p_error_message, @p_stack_trace, @p_operation_name)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_start_date", NpgsqlDbType.Date, dTOErrorLog.StartDate == DateTime.MaxValue ? (object)DBNull.Value : (object)startDate);
                        command.Parameters.AddWithValue("@p_end_date", NpgsqlDbType.Date, dTOErrorLog.EndDate == DateTime.MaxValue ? (object)DBNull.Value : (object)endDate);
                        command.Parameters.AddWithValue("@p_error_message", NpgsqlDbType.Text, (object)dTOErrorLog.ErrorMessage ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_stack_trace", NpgsqlDbType.Text, (object)dTOErrorLog.StackTrace ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_operation_name", NpgsqlDbType.Text, (object)dTOErrorLog.OperationName ?? DBNull.Value);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);
                        foreach (DataRow item in dataTable.Rows)
                        {
                            DTOErrorLog errorLog = new DTOErrorLog
                            {
                                ErrorId = (int)item["id"],
                                ErrorDateTime = (DateTime)item["errordatetime"],
                                ErrorMessage = item["errormessage"].ToString(),
                                StackTrace = item["stacktrace"].ToString(),
                                OperationName = item["operationname"].ToString(),
                            };
                            errorLogs.Add(errorLog);
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
            return errorLogs;
        }

        /// <summary>
        /// Insert SP caller for ErrorLog
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="methodName"></param>
        public void InsertErrorLog(Exception ex, string methodName)
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
        /// Delete SP caller for ErrorLog
        /// </summary>
        /// <param name="dTOErrorLog"></param>
        /// <returns></returns>
        public bool DeleteErrorLog(DTOErrorLog dTOErrorLog)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_DeleteErrorLog";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_id)";

                    using (NpgsqlCommand commandDeleteAccount = new NpgsqlCommand(sqlQuery, connection))
                    {
                        commandDeleteAccount.Parameters.AddWithValue("@p_account_no", dTOErrorLog.ErrorId);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandDeleteAccount);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            bool success = (bool)dataTable.Rows[0]["func_DeleteErrorLog"];

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

