using System.Data;
using System.Reflection;
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

                    string sqlSelectBakiye = $"SELECT * FROM {functionName}(@hesapNo)";

                    using (NpgsqlCommand commandSelectBakiye = new NpgsqlCommand(sqlSelectBakiye, connection))
                    {
                        commandSelectBakiye.Parameters.AddWithValue("@hesapNo", accountInfo.HesapNo);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandSelectBakiye);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        accountInfo = new DTOAccountInfo
                        {
                            HesapNo = (long)dataTable.Rows[0]["hesap_no"],
                            Bakiye = (decimal)dataTable.Rows[0]["bakiye"],
                            MusteriId = (int)dataTable.Rows[0]["musteri_id"],
                            DovizCins = (int)dataTable.Rows[0]["doviz_cins"],
                            HesapAdi = dataTable.Rows[0]["hesap_adi"].ToString(),
                        };

                        //object result = commandSelectBakiye.ExecuteScalar();
                        if (dataTable.Rows.Count == 0)
                        {
                            Console.WriteLine("\nHesap bulunamadı.");
                            return accountInfo;
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

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_bakiye, @p_musteri_id, @p_doviz_cins, @p_hesap_adi)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_bakiye", NpgsqlDbType.Numeric, dTOAccount.Bakiye);
                        command.Parameters.AddWithValue("@p_musteri_id", NpgsqlDbType.Integer, dTOAccount.MusteriId);
                        command.Parameters.AddWithValue("@p_doviz_cins", NpgsqlDbType.Integer, dTOAccount.DovizCins);
                        command.Parameters.AddWithValue("@p_hesap_adi", NpgsqlDbType.Varchar, (object)dTOAccount.HesapAdi ?? DBNull.Value);

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
                                HesapAdi = item["hesap_adi"].ToString() //!= DBNull.Value ? item["hesap_adi"].ToString() : null
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

                    string sqlQuery = $"CALL {procedureName}(@p_bakiye, @p_musteriid, @p_doviz_cins, @p_hesap_adi)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_bakiye", dTOAccount.Bakiye);
                        command.Parameters.AddWithValue("@p_musteriid", dTOAccount.MusteriId);
                        command.Parameters.AddWithValue("@p_doviz_cins", dTOAccount.DovizCins);
                        command.Parameters.AddWithValue("@p_hesap_adi", dTOAccount.HesapAdi);

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

                    string sqlQuery = $"CALL {functionName}(@p_hesapno, @p_bakiye, @p_musteriid, @p_doviz_cins, @p_hesap_adi)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_hesapno", dTOAccount.HesapNo);
                        command.Parameters.AddWithValue("@p_bakiye", dTOAccount.Bakiye);
                        command.Parameters.AddWithValue("@p_musteriid", dTOAccount.MusteriId);
                        command.Parameters.AddWithValue("@p_doviz_cins", dTOAccount.DovizCins);
                        command.Parameters.AddWithValue("@p_hesap_adi", dTOAccount.HesapAdi);

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
        /// Hesap Tablosundan Veri Siler
        /// </summary>
        /// <param name="hesapNo">Müşteri hesap no</param>
        /// <returns></returns>
        public bool DeleteAccountInfo(DTOAccountInfo dTOAccount)
        {
            dTOAccount = ReadAccountInfo(dTOAccount);

            if (dTOAccount != null && dTOAccount.Bakiye == 0)
            {
                try
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                    {
                        connection.Open();

                        // Delete the account using func_DeleteAccountInfo function
                        string functionName = "func_DeleteAccountInfo";

                        string sqlQuery = $"SELECT * FROM {functionName}(@hesap_no)";

                        using (NpgsqlCommand commandDeleteHesap = new NpgsqlCommand(sqlQuery, connection))
                        {
                            commandDeleteHesap.Parameters.AddWithValue("@hesap_no", dTOAccount.HesapNo);

                            NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandDeleteHesap);
                            DataTable dataTable = new DataTable();

                            npgsqlDataAdapter.Fill(dataTable);

                            // Check the result in the DataTable
                            if (dataTable.Rows.Count > 0)
                            {
                                bool success = (bool)dataTable.Rows[0]["func_deleteaccountinfo"];

                                if (success)
                                {
                                    Console.WriteLine("\nHesap başarıyla silindi.");
                                    return true;
                                }
                                else
                                {
                                    Console.WriteLine("\nHesap silinemedi. Lütfen tekrar deneyin.");
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
            }
            else
            {
                Console.WriteLine("\nHesap bakiyesi 0 değil. Lütfen bakiyeyi başka bir hesaba aktarın.");
            }
            return false;
        }
    }
}