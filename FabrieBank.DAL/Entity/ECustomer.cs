using System.Data;
using System.Reflection;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL;
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

        public void CreateCustomer(DTOCustomer customer)
        {
            dataAccessLayer.CreateCustomer(customer);
        }

        public int GetNextMusteriId(NpgsqlConnection connection)
        {
            return dataAccessLayer.GetNextMusteriId(connection);
        }

        public bool IsCustomerExists(NpgsqlConnection connection, long tckn)
        {
            return dataAccessLayer.IsCustomerExists(connection, tckn);
        }

        /// <summary>
        /// Müşteri Tablosundan Tek Veri Döndürür
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

                        DTOCustomer dTOCustomer = new DTOCustomer
                        {
                            MusteriId = (int)dataTable.Rows[0]["musteri_id"],
                            Ad = dataTable.Rows[0]["ad"].ToString(),
                            Soyad = dataTable.Rows[0]["soyad"].ToString(),
                            Tckn = (long)dataTable.Rows[0]["tckn"],
                            Sifre = (int)dataTable.Rows[0]["sifre"],
                            TelNo = (long)dataTable.Rows[0]["tel_no"],
                            Email = dataTable.Rows[0]["email"].ToString(),
                        };

                        if (dataTable.Rows.Count == 0)
                        {
                            Console.WriteLine("\nHesap bulunamadı.");
                            return dTOCustomer;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                dataAccessLayer.LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return customer;
        }

        /// <summary>
        /// Müşteri Tablosundan Liste Döndürür
        /// </summary>
        /// <param name="dTOAccount"></param>
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

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_ad, @p_soyad, @p_tel_no, @p_email)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_ad", NpgsqlDbType.Varchar, (object)customer.Ad ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_soyad", NpgsqlDbType.Varchar, (object)customer.Soyad ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_tel_no", NpgsqlDbType.Bigint, customer.TelNo);
                        command.Parameters.AddWithValue("@p_email", NpgsqlDbType.Varchar, (object)customer.Email ?? DBNull.Value);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);
                        foreach (DataRow item in dataTable.Rows)
                        {
                            DTOCustomer dTOCustomer = new DTOCustomer
                            {
                                MusteriId = (int)item["musteri_id"],
                                Ad = item["ad"].ToString(),
                                Soyad = item["soyad"].ToString(),
                                Tckn = (long)item["tckn"],
                                Sifre = (int)item["sifre"],
                                TelNo = (long)item["tel_no"],
                                Email = item["email"].ToString(),
                            };
                            accountsList.Add(dTOCustomer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                dataAccessLayer.LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return accountsList;
        }

        /// <summary>
        /// Musteri_Bilgi Tablosuna Veri Gönderir
        /// </summary>
        /// <param name="dTOAccount"></param>
        /// <returns></returns>
        public bool InsertCustomer(DTOCustomer customer)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_InsertCustomer";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_ad, @p_soyad, @p_tckn, @p_sifre ,@p_tel_no, @p_email)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_ad", NpgsqlDbType.Varchar, (object)customer.Ad ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_soyad", NpgsqlDbType.Varchar, (object)customer.Soyad ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_tckn", NpgsqlDbType.Bigint, customer.Tckn);
                        command.Parameters.AddWithValue("@p_sifre", NpgsqlDbType.Integer, customer.Sifre);
                        command.Parameters.AddWithValue("@p_tel_no", NpgsqlDbType.Bigint, customer.TelNo);
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
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                dataAccessLayer.LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return false;
        }

        /// <summary>
        /// Musteri_Bilgi Tablosundaki Verileri Yeniler
        /// </summary>
        /// <param name="dTOAccount"></param>
        /// <returns></returns>
        public bool UpdateCustomer(DTOCustomer customer)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_UpdateCustomer";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_ad, @p_soyad, @p_tckn, @p_sifre ,@p_tel_no, @p_email)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_ad", NpgsqlDbType.Varchar, (object)customer.Ad ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_soyad", NpgsqlDbType.Varchar, (object)customer.Soyad ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_tckn", NpgsqlDbType.Bigint, customer.Tckn);
                        command.Parameters.AddWithValue("@p_sifre", NpgsqlDbType.Integer, customer.Sifre);
                        command.Parameters.AddWithValue("@p_tel_no", NpgsqlDbType.Bigint, customer.TelNo);
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
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                dataAccessLayer.LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return false;
        }

        /// <summary>
        /// Musteri_Bilgi Tablosundan Veri Siler
        /// </summary>
        /// <param name="hesapNo">Müşteri hesap no</param>
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
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                dataAccessLayer.LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
            return false;
        }
    }
}