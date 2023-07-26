using System.Data;
using System.Reflection;
using FabrieBank.Common;
using FabrieBank.Common.Enums;
using FabrieBank.DAL;
using Npgsql;
using NpgsqlTypes;

namespace FabrieBank.Entity
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

        public bool UpdateAccountInfo(DTOAccountInfo dTOAccount)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_UpdateAccountInfo";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_hesapno, @p_bakiye, @p_musteriid, @p_doviz_cins, @p_hesap_adi)";

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
                        command.Parameters.AddWithValue("@p_bakiye", dTOAccount.Bakiye);
                        command.Parameters.AddWithValue("@p_musteri_id", dTOAccount.MusteriId);
                        command.Parameters.AddWithValue("@p_doviz_cins", dTOAccount.DovizCins);
                        command.Parameters.AddWithValue("@p_hesap_adi", dTOAccount.HesapAdi);


                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);
                        foreach (DataRow item in dataTable.Rows)
                        {
                            DTOAccountInfo dTOAccountInfo = new DTOAccountInfo
                            {
                                HesapNo = (long)item["hesap_no"],
                                Bakiye = (decimal)item["bakiye"],
                                DovizCins = (EnumDovizCinsleri.DovizCinsleri)item["doviz_cins"],
                                HesapAdi = item["hesap_adi"].ToString(),
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
            return new List<DTOAccountInfo>();
        }

        public bool DeleteAccount(long hesapNo)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    // Delete the account usp_DelHesap
                    string procedureName = "usp_DeleteAccountInfo";

                    using (NpgsqlCommand commandDeleteHesap = new NpgsqlCommand(procedureName, connection))
                    {
                        commandDeleteHesap.CommandType = CommandType.StoredProcedure;

                        // Add the output parameter
                        NpgsqlParameter successParam = new NpgsqlParameter("success", NpgsqlDbType.Boolean);
                        successParam.Direction = ParameterDirection.Output;
                        commandDeleteHesap.Parameters.Add(successParam);

                        commandDeleteHesap.Parameters.AddWithValue("hesap_no", hesapNo);

                        commandDeleteHesap.ExecuteNonQuery();

                        // Check the output parameter to determine if the delete was successful
                        bool success = (bool)successParam.Value;

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

        public bool ReadAccountInfo(long hesapNo)
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
                        commandSelectBakiye.Parameters.AddWithValue("@hesapNo", hesapNo);

                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(commandSelectBakiye);
                        DataTable dataTable = new DataTable();

                        npgsqlDataAdapter.Fill(dataTable);

                        //object result = commandSelectBakiye.ExecuteScalar();
                        if (dataTable.Rows.Count == 0)
                        {
                            Console.WriteLine("\nHesap bulunamadı.");
                            return false;
                        }

                        decimal bakiye = Convert.ToDecimal(dataTable.Rows[0][0]);
                        if (bakiye != 0)
                        {
                            Console.WriteLine("\nHesap bakiyesi 0 değil. Lütfen bakiyeyi başka bir hesaba aktarın.");
                            return false;
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

        public bool InsertAccountInfo(DTOAccountInfo dTOAccount)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "usp_InsertAccountInfo";

                    string sqlQuery = $"SELECT * FROM {functionName}(@p_hesapno, @p_bakiye, @p_musteriid, @p_doviz_cins, @p_hesap_adi)";

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
    }
}