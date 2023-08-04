using System.Data;
using FabrieBank.DAL.Common.DTOs;
using Npgsql;
using NpgsqlTypes;

namespace FabrieBank.DAL.Entity
{
    public class ECustomer
    {
        private ErrorLoggerDB errorLogger;
        private DataAccessLayer dataAccessLayer;
        private NpgsqlConnectionStringBuilder database;

        public ECustomer()
        {
            errorLogger = new ErrorLoggerDB();
            dataAccessLayer = new DataAccessLayer();
            database = dataAccessLayer.CallDB();
        }

        /// <summary>
        /// Müşteri Oluşturur
        /// </summary>
        /// <param name="customer"></param>
        public void CreateCustomer(DTOCustomer customer)
        {

            DTOCustomer existingCustomer = ReadCustomer(customer);

            if (existingCustomer != null)
            {
                Console.WriteLine("\nA customer with the same TCKN already exists.\n");
            }
            else
            {
                _ = InsertCustomer(customer);
                Console.WriteLine("\nA customer created succesfully.\n");
            }
        }

        /// <summary>
        /// Giriş işlemi yapar
        /// </summary>
        /// <param name="tckn"></param>
        /// <param name="sifre"></param>
        /// <returns></returns>
        public DTOCustomer LogIn(long tckn, int sifre)
        {
            DTOCustomer customer = ReadCustomer(new DTOCustomer { Tckn = tckn });

            if (customer != null && customer.Sifre == sifre)
            {
                return customer;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// İletişim bilgileri güncelleme
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool UpdatePersonelInfo(DTOCustomer customer)
        {
            UpdateCustomer(customer);
            return true;
        }

        /// <summary>
        /// Şifre değiştirir
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool ChangePassword(DTOCustomer customer)
        {
            UpdateCustomer(customer);
            return true;
        }

        /// <summary>
        /// Şifremi unuttum işlemleri
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="email"></param>
        /// <param name="temporaryPassword"></param>
        /// <returns></returns>
        public bool ForgotPassword(DTOCustomer customer, string email, int temporaryPassword)
        {
            customer = ReadCustomer(customer);

            if (customer.Email != email)
            {
                Console.WriteLine("Girmiş olduğunuz bilgiler uyuşmuyor. Lütfen tekrar deneyiniz.");
                return false;
            }
            else
            {
                DTOCustomer dTOCustomer = new DTOCustomer()
                {
                    MusteriId = customer.MusteriId,
                    Ad = customer.Ad,
                    Soyad = customer.Soyad,
                    Sifre = temporaryPassword,
                    TelNo = customer.TelNo,
                    Email = customer.Email
                };

                UpdateCustomer(dTOCustomer);
                Console.WriteLine($"\nGeçici şifreniz başarıyla oluşturuldu. Şifreniz: {temporaryPassword}");
                return true;
            }
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

                        if (dataTable.Rows.Count > 0)
                        {
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
                errorLogger.LogAndHandleError(ex);
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
                errorLogger.LogAndHandleError(ex);
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

                    string sqlQuery = $"CALL {functionName}(@p_ad, @p_soyad, @p_tckn, @p_sifre ,@p_tel_no, @p_email)";

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
                errorLogger.LogAndHandleError(ex);
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

                    string sqlQuery = $"CALL {functionName}(@p_musteri_id, @p_ad, @p_soyad, @p_sifre ,@p_tel_no, @p_email)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@p_musteri_id", NpgsqlDbType.Integer, customer.MusteriId);
                        command.Parameters.AddWithValue("@p_ad", NpgsqlDbType.Varchar, (object)customer.Ad ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_soyad", NpgsqlDbType.Varchar, (object)customer.Soyad ?? DBNull.Value);
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
                errorLogger.LogAndHandleError(ex);
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
                errorLogger.LogAndHandleError(ex);
            }
            return false;
        }
    }
}