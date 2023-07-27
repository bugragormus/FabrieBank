using System.Data;
using System.Reflection;
using FabrieBank.Common;
using FabrieBank.DAL;
using Npgsql;
using NpgsqlTypes;

namespace FabrieBank.Entity
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
        public List<DTOAccountInfo> ReadListAccountInfo(DTOAccountInfo dTOAccount)
        {
            List<DTOAccountInfo> accountsList = new List<DTOAccountInfo>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadListAccountInfo";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_bakiye, @p_musteri_id, @p_doviz_cins)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_bakiye", NpgsqlDbType.Numeric, dTOAccount.Bakiye);
                        command.Parameters.AddWithValue("@p_musteri_id", NpgsqlDbType.Integer, dTOAccount.MusteriId);
                        command.Parameters.AddWithValue("@p_doviz_cins", NpgsqlDbType.Integer, dTOAccount.DovizCins);
                        //command.Parameters.AddWithValue("@p_hesap_adi", NpgsqlDbType.Varchar ,dTOAccount.HesapAdi);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);
                        foreach (DataRow item in dataTable.Rows)
                        {
                            DTOAccountInfo dTOAccountInfo = new DTOAccountInfo
                            {
                                HesapNo = (long)item["hesap_no"],
                                Bakiye = (decimal)item["bakiye"],
                                MusteriId = (int)item["musteri_id"],
                                DovizCins = (int)item["doviz_cins"],
                                HesapAdi = item["hesap_adi"] != DBNull.Value ? item["hesap_adi"].ToString() : null
                            };
                            accountsList.Add(dTOAccountInfo);
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
    }
}