using System.Data;
using System.Reflection;
using FabrieBank.DAL.Common.DTOs;
using Npgsql;
using NpgsqlTypes;

namespace FabrieBank.DAL.Entity
{
    public class ECustomer
    {
        private DataAccessLayer dataAccessLayer;
        private NpgsqlConnectionStringBuilder database;

        public ECustomer()
        {
            dataAccessLayer = new DataAccessLayer();
            database = dataAccessLayer.CallDB();
        }

        /// <summary>
        /// Read By Id SP caller for Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public DTOCustomer ReadCustomer(DTOCustomer customer)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadCustomer";

                    string vsql = $"SELECT * FROM {functionName}(@tckn)";

                    using (NpgsqlCommand command = new NpgsqlCommand(vsql, connection))
                    {
                        command.Parameters.AddWithValue("@tckn", customer.Tckn);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            DTOCustomer dTOCustomer = new DTOCustomer
                            {
                                CustomerId = (int)dataTable.Rows[0]["customer_id"],
                                Name = dataTable.Rows[0]["name"].ToString(),
                                Lastname = dataTable.Rows[0]["lastname"].ToString(),
                                Tckn = (long)dataTable.Rows[0]["tckn"],
                                Password = dataTable.Rows[0]["password"].ToString(),
                                CellNo = (long)dataTable.Rows[0]["cell_no"],
                                Email = dataTable.Rows[0]["email"].ToString(),
                            };

                            return dTOCustomer;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return customer;
        }

        /// <summary>
        /// Read List SP caller for Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public List<DTOCustomer> ReadListCustomer(DTOCustomer customer)
        {
            List<DTOCustomer> accountsList = new List<DTOCustomer>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadListCustomer";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_name, @p_lastname, @p_cell_no, @p_email)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_name", NpgsqlDbType.Varchar, (object)customer.Name ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_lastname", NpgsqlDbType.Varchar, (object)customer.Lastname ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_cell_no", NpgsqlDbType.Bigint, customer.CellNo);
                        command.Parameters.AddWithValue("@p_email", NpgsqlDbType.Varchar, (object)customer.Email ?? DBNull.Value);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);
                        foreach (DataRow item in dataTable.Rows)
                        {
                            DTOCustomer dTOCustomer = new DTOCustomer
                            {
                                CustomerId = (int)dataTable.Rows[0]["customer_id"],
                                Name = dataTable.Rows[0]["name"].ToString(),
                                Lastname = dataTable.Rows[0]["lastname"].ToString(),
                                Tckn = (long)dataTable.Rows[0]["tckn"],
                                Password = dataTable.Rows[0]["password"].ToString(),
                                CellNo = (long)dataTable.Rows[0]["cell_no"],
                                Email = dataTable.Rows[0]["email"].ToString(),
                            };
                            accountsList.Add(dTOCustomer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return accountsList;
        }

        /// <summary>
        /// Insert SP caller for Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool InsertCustomer(DTOCustomer customer)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_InsertCustomer";

                    string sqlQuery = $"CALL {functionName}(@p_name, @p_lastname, @p_tckn, @p_password ,@p_cell_no, @p_email)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_name", NpgsqlDbType.Varchar, (object)customer.Name ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_lastname", NpgsqlDbType.Varchar, (object)customer.Lastname ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_tckn", NpgsqlDbType.Bigint, customer.Tckn);
                        command.Parameters.AddWithValue("@p_password", NpgsqlDbType.Varchar, (object)customer.Password ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_cell_no", NpgsqlDbType.Bigint, customer.CellNo);
                        command.Parameters.AddWithValue("@p_email", NpgsqlDbType.Varchar, (object)customer.Email ?? DBNull.Value);

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
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return false;
        }

        /// <summary>
        /// Update SP caller for Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool UpdateCustomer(DTOCustomer customer)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_UpdateCustomer";

                    string sqlQuery = $"CALL {functionName}(@p_customer_id, @p_name, @p_lastname, @p_password ,@p_cell_no, @p_email)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_customer_id", NpgsqlDbType.Integer, customer.CustomerId);
                        command.Parameters.AddWithValue("@p_name", NpgsqlDbType.Varchar, (object)customer.Name ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_lastname", NpgsqlDbType.Varchar, (object)customer.Lastname ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_password", NpgsqlDbType.Varchar, (object)customer.Password ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_cell_no", NpgsqlDbType.Bigint, customer.CellNo);
                        command.Parameters.AddWithValue("@p_email", NpgsqlDbType.Varchar, (object)customer.Email ?? DBNull.Value);

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
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return false;
        }

        /// <summary>
        /// Delete SP caller for Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool DeleteCustomer(DTOCustomer customer)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    // Delete the account using func_DeleteAccountInfo function
                    string functionName = "func_DeleteCustomer";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_tckn)";

                    using (NpgsqlCommand commandDeleteHesap = new NpgsqlCommand(sqlQuery, connection))
                    {
                        commandDeleteHesap.Parameters.AddWithValue("@p_tckn", customer.Tckn);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandDeleteHesap);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        // Check the result in the DataTable
                        if (dataTable.Rows.Count > 0)
                        {
                            bool success = (bool)dataTable.Rows[0]["func_DeleteCustomer"];

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
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return false;
        }
    }
}