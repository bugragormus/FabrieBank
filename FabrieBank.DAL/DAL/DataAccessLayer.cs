using FabrieBank.Common;
using FabrieBank.Common.Enums;
using System.Reflection;
using Npgsql;
using FabrieBank.Common.DTOs;
using NpgsqlTypes;
using System.Data;

namespace FabrieBank.DAL
{
    public class DataAccessLayer
    {
        private NpgsqlConnectionStringBuilder database;

        public DataAccessLayer()
        {
            database = CallDB();
        }

        /*
        ************************************************************************************************
        *******************************************DB Bağlanma******************************************
        ************************************************************************************************
        */

        private NpgsqlConnectionStringBuilder CallDB()
        {
            try
            {
                NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

                builder.Host = "localhost";
                builder.Username = "postgres";
                builder.Password = "43324332";
                builder.Database = "FabrieBank";

                return builder;
            }
            catch (Exception ex)
            {

                return new NpgsqlConnectionStringBuilder();
            }
        }

        /*
        ************************************************************************************************
        ****************************************ErrorLoggerDB.cs****************************************
        ************************************************************************************************
        */

        public void LogError(Exception ex, string methodName)
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

        /*
        ************************************************************************************************
        ******************************************AccInfoDB.cs******************************************
        ************************************************************************************************
        */

        public List<DTOAccountInfo> GetAccountInfo(int musteriId)
        {
            List<DTOAccountInfo> accountInfos = new List<DTOAccountInfo>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand("usp_GetAccountInfo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@musteri_id", musteriId);

                        // Define OUT parameters to retrieve the result data
                        command.Parameters.Add(new NpgsqlParameter("hesap_no", NpgsqlDbType.Bigint) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new NpgsqlParameter("bakiye", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new NpgsqlParameter("doviz_cins", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new NpgsqlParameter("hesap_adi", NpgsqlDbType.Text) { Direction = ParameterDirection.Output });

                        command.ExecuteNonQuery();

                        // Retrieve the result data from OUT parameters
                        long hesapNo = Convert.ToInt64(command.Parameters["hesap_no"].Value);
                        decimal bakiye = Convert.ToDecimal(command.Parameters["bakiye"].Value);
                        int dovizCins = Convert.ToInt32(command.Parameters["doviz_cins"].Value);
                        string hesapAdi = command.Parameters["hesap_adi"].Value.ToString();

                        // Create DTOAccountInfo object with retrieved data
                        DTOAccountInfo dTOAccountInfo = new DTOAccountInfo
                        {
                            HesapNo = hesapNo,
                            Bakiye = bakiye,
                            MusteriId = musteriId,
                            DovizCins = (EnumDovizCinsleri.DovizCinsleri)dovizCins,
                            HesapAdi = hesapAdi,
                        };

                        accountInfos.Add(dTOAccountInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }

            return accountInfos;
        }

        public bool DeleteAccount(long hesapNo)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    // Check account balance
                    string sqlSelectBakiye = "SELECT Bakiye FROM public.Hesap WHERE HesapNo = @hesapNo";
                    using (NpgsqlCommand commandSelectBakiye = new NpgsqlCommand(sqlSelectBakiye, connection))
                    {
                        commandSelectBakiye.Parameters.AddWithValue("@hesapNo", hesapNo);

                        object result = commandSelectBakiye.ExecuteScalar();
                        if (result == null)
                        {
                            Console.WriteLine("\nHesap bulunamadı.");
                            return false;
                        }

                        decimal bakiye = Convert.ToDecimal(result);
                        if (bakiye != 0)
                        {
                            Console.WriteLine("\nHesap bakiyesi 0 değil. Lütfen bakiyeyi başka bir hesaba aktarın.");
                            return false;
                        }
                    }

                    // Delete the account
                    string sqlDeleteHesap = "DELETE FROM public.Hesap WHERE HesapNo = @hesapNo";
                    using (NpgsqlCommand commandDeleteHesap = new NpgsqlCommand(sqlDeleteHesap, connection))
                    {
                        commandDeleteHesap.Parameters.AddWithValue("@hesapNo", hesapNo);

                        int affectedRows = commandDeleteHesap.ExecuteNonQuery();
                        if (affectedRows > 0)
                        {
                            Console.WriteLine("\nHesap başarıyla silindi.");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("\nHesap silinemedi. Lütfen tekrar deneyin.");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return false;
            }
        }

        /*
        ************************************************************************************************
        **********************************************ATM.cs********************************************
        ************************************************************************************************
        */

        public void Deposit(long hesapNo, decimal bakiye)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string sqlSelect = "SELECT Bakiye FROM public.Hesap WHERE HesapNo = @hesapNo";
                    string sqlUpdate = "UPDATE public.Hesap SET Bakiye = Bakiye + @bakiye WHERE HesapNo = @hesapNo";

                    using (NpgsqlCommand commandSelect = new NpgsqlCommand(sqlSelect, connection))
                    {
                        commandSelect.Parameters.AddWithValue("@hesapNo", hesapNo);

                        decimal eskiBakiye = Convert.ToDecimal(commandSelect.ExecuteScalar());
                        decimal yeniBakiye = eskiBakiye + bakiye;

                        using (NpgsqlCommand commandUpdate = new NpgsqlCommand(sqlUpdate, connection))
                        {
                            commandUpdate.Parameters.AddWithValue("@bakiye", bakiye);
                            commandUpdate.Parameters.AddWithValue("@hesapNo", hesapNo);

                            int rowsAffected = commandUpdate.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {

                                // Log the successful deposit
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    AccountNumber = hesapNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Success,
                                    Amount = yeniBakiye - eskiBakiye,
                                    OldBalance = eskiBakiye,
                                    NewBalance = yeniBakiye,
                                    Timestamp = DateTime.Now
                                };

                                LogTransaction(transactionLog);

                                Console.WriteLine("\nPara yatırma işlemi başarılı.");
                                Console.WriteLine($"Eski bakiye: {eskiBakiye}");
                                Console.WriteLine($"Yeni bakiye: {yeniBakiye}");
                            }
                            else
                            {

                                // Log the failed deposit
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    AccountNumber = hesapNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Failed,
                                    Amount = yeniBakiye - eskiBakiye,
                                    OldBalance = eskiBakiye,
                                    NewBalance = yeniBakiye,
                                    Timestamp = DateTime.Now
                                };

                                LogTransaction(transactionLog);

                                Console.WriteLine("\nPara yatırma işlemi başarısız.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine("An error occurred while performing ParaYatirma operation. Please try again later.");
            }
        }

        public void Withdraw(long hesapNo, decimal bakiye)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string sqlSelect = "SELECT Bakiye FROM public.Hesap WHERE HesapNo = @hesapNo";
                    string sqlUpdate = "UPDATE public.Hesap SET Bakiye = Bakiye - @bakiye WHERE HesapNo = @hesapNo";

                    using (NpgsqlCommand commandSelect = new NpgsqlCommand(sqlSelect, connection))
                    {
                        commandSelect.Parameters.AddWithValue("@hesapNo", hesapNo);

                        decimal eskiBakiye = Convert.ToDecimal(commandSelect.ExecuteScalar());
                        decimal yeniBakiye = eskiBakiye - bakiye;

                        if (yeniBakiye >= 0)
                        {
                            using (NpgsqlCommand commandUpdate = new NpgsqlCommand(sqlUpdate, connection))
                            {
                                commandUpdate.Parameters.AddWithValue("@bakiye", bakiye);
                                commandUpdate.Parameters.AddWithValue("@hesapNo", hesapNo);

                                int rowsAffected = commandUpdate.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    // Log the successful withdrawal
                                    DTOTransactionLog transactionLog = new DTOTransactionLog
                                    {
                                        AccountNumber = hesapNo,
                                        TransactionType = EnumTransactionType.Withdrawal,
                                        TransactionStatus = EnumTransactionStatus.Success,
                                        Amount = eskiBakiye - yeniBakiye,
                                        OldBalance = eskiBakiye,
                                        NewBalance = yeniBakiye,
                                        Timestamp = DateTime.Now
                                    };

                                    LogTransaction(transactionLog);

                                    Console.WriteLine("\nPara çekme işlemi başarılı.");
                                    Console.WriteLine($"Eski bakiye: {eskiBakiye}");
                                    Console.WriteLine($"Yeni bakiye: {yeniBakiye}");
                                }
                                else
                                {

                                    // Log the failed withdraw
                                    DTOTransactionLog transactionLog = new DTOTransactionLog
                                    {
                                        AccountNumber = hesapNo,
                                        TransactionType = EnumTransactionType.Withdrawal,
                                        TransactionStatus = EnumTransactionStatus.Failed,
                                        Amount = eskiBakiye - yeniBakiye,
                                        OldBalance = eskiBakiye,
                                        NewBalance = yeniBakiye,
                                        Timestamp = DateTime.Now
                                    };

                                    LogTransaction(transactionLog);

                                    Console.WriteLine("\nPara çekme işlemi başarısız.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n\nYetersiz bakiye. Para çekme işlemi gerçekleştirilemedi.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine("An error occurred while performing ParaCekme operation. Please try again later.");
            }
        }

        /*
        ************************************************************************************************
        ****************************************CreateAccountDB.cs**************************************
        ************************************************************************************************
        */

        public void CreateAccount(int musteriId, int dovizCinsi, string hesapAdi)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    // Get account number and increment
                    string hesapNumarasi = GetAndIncrementHesapNumarasi(dovizCinsi, connection);

                    // Convert hesapNumarasi to long
                    long hesapNo = long.Parse(hesapNumarasi);

                    // Add to Hesap table
                    string sql = "INSERT INTO public.Hesap (hesapno, bakiye, musteriid, dovizcins, hesapadi) VALUES (@hesapNo, 0, @musteriId, @dovizCinsi, @hesapAdi)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@hesapNo", hesapNo);
                        command.Parameters.AddWithValue("@musteriId", musteriId);
                        command.Parameters.AddWithValue("@dovizCinsi", dovizCinsi);
                        command.Parameters.AddWithValue("@hesapAdi", hesapAdi);

                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine($"\n'{hesapNumarasi}' Numaralı yeni hesap oluşturuldu.\n");
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        public string GetAndIncrementHesapNumarasi(int dovizCinsi, NpgsqlConnection connection)
        {
            string sqlSelect = "SELECT HesapNumarasi FROM public.Hesap_No WHERE DovizCinsi = @dovizCinsi";
            string sqlUpdate = "UPDATE public.Hesap_No SET HesapNumarasi = HesapNumarasi + 1 WHERE DovizCinsi = @dovizCinsi";

            string hesapNumarasi = string.Empty;

            try
            {
                using (NpgsqlCommand commandSelect = new NpgsqlCommand(sqlSelect, connection))
                {
                    commandSelect.Parameters.AddWithValue("@dovizCinsi", dovizCinsi);

                    using (NpgsqlDataReader reader = commandSelect.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            hesapNumarasi = reader.GetInt64(0).ToString();
                        }
                    }
                }

                using (NpgsqlCommand commandUpdate = new NpgsqlCommand(sqlUpdate, connection))
                {
                    commandUpdate.Parameters.AddWithValue("@dovizCinsi", dovizCinsi);
                    commandUpdate.ExecuteNonQuery();
                }

                return hesapNumarasi;
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return hesapNumarasi;
            }
        }

        /*
        ************************************************************************************************
        ***************************************CreateCustomerDB.cs**************************************
        ************************************************************************************************
        */

        public void CreateCustomer(DTOCustomer customer)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    if (IsCustomerExists(connection, customer.Tckn))
                    {
                        Console.WriteLine("\nCustomer already exists.");
                        return;
                    }

                    int nextMusteriId = GetNextMusteriId(connection);

                    string enableIdentityInsertSql = "ALTER TABLE public.Musteri_Bilgi ADD COLUMN IF NOT EXISTS MusteriId SERIAL PRIMARY KEY";
                    using (NpgsqlCommand enableIdentityInsertCommand = new NpgsqlCommand(enableIdentityInsertSql, connection))
                    {
                        enableIdentityInsertCommand.ExecuteNonQuery();
                    }

                    string sql = "INSERT INTO public.Musteri_Bilgi (MusteriId, Ad, Soyad, Tckn, Sifre, TelNo, Email) VALUES (@musteriId, @ad, @soyad, @tckn, @sifre, @telNo, @email)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@musteriId", nextMusteriId);
                        command.Parameters.AddWithValue("@ad", customer.Ad);
                        command.Parameters.AddWithValue("@soyad", customer.Soyad);
                        command.Parameters.AddWithValue("@tckn", customer.Tckn);
                        command.Parameters.AddWithValue("@sifre", customer.Sifre);
                        command.Parameters.AddWithValue("@telNo", customer.TelNo);
                        command.Parameters.AddWithValue("@email", customer.Email);

                        command.ExecuteNonQuery();
                    }
                    Console.Clear();
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        public int GetNextMusteriId(NpgsqlConnection connection)
        {
            string sql = "SELECT COALESCE(MAX(MusteriId), 0) + 1 FROM public.Musteri_Bilgi";

            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    int nextMusteriId = Convert.ToInt32(command.ExecuteScalar());
                    return nextMusteriId;
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return GetNextMusteriId(connection);
            }
        }

        public bool IsCustomerExists(NpgsqlConnection connection, long tckn)
        {
            string sql = "SELECT COUNT(*) FROM public.Musteri_Bilgi WHERE Tckn = @tckn";

            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tckn", tckn);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return false;
            }
        }

        /*
        ************************************************************************************************
        *******************************************LogInDB.cs*******************************************
        ************************************************************************************************
        */

        public DTOCustomer LogIn(long tckn, int sifre)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM public.Musteri_Bilgi WHERE Tckn = @tckn AND Sifre = @sifre";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tckn", tckn);
                        command.Parameters.AddWithValue("@sifre", sifre);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DTOCustomer customer = new DTOCustomer()
                                {
                                    MusteriId = reader.GetInt32(0),
                                    Ad = reader.GetString(1),
                                    Soyad = reader.GetString(2),
                                    Tckn = reader.GetInt64(3),
                                    Sifre = reader.GetInt32(4),
                                    TelNo = reader.GetInt64(5),
                                    Email = reader.GetString(6)
                                };

                                return customer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }

            // If no matching user is found, return null to indicate login failure
            return null;
        }

        public bool UpdatePersonelInfo(int musteriId, long telNo, string email)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string sql = "UPDATE public.Musteri_Bilgi SET TelNo = @telNo, Email = @email WHERE MusteriId = @musteriId";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@telNo", telNo);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@musteriId", musteriId);

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return false;
            }
        }

        public bool IsCredentialsValid(long tckn, int sifre)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string sql = "SELECT COUNT(*) FROM public.Musteri_Bilgi WHERE Tckn = @tckn AND Sifre = @sifre";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tckn", tckn);
                        command.Parameters.AddWithValue("@sifre", sifre);

                        int result = Convert.ToInt32(command.ExecuteScalar());

                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return IsCredentialsValid(tckn, sifre);
            }
        }

        public bool ForgotPassword(long tckn, string email, int temporaryPassword)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    // Check if the user with the given TCKN and email exists
                    string sqlSelect = "SELECT MusteriId FROM Musteri_Bilgi WHERE Tckn = @tckn AND Email = @email";

                    using (NpgsqlCommand commandSelect = new NpgsqlCommand(sqlSelect, connection))
                    {
                        commandSelect.Parameters.AddWithValue("@tckn", tckn);
                        commandSelect.Parameters.AddWithValue("@email", email);

                        object result = commandSelect.ExecuteScalar();

                        if (result == null)
                        {
                            Console.WriteLine("\nHatalı TCKN veya e-posta adresi. Şifre sıfırlama işlemi başarısız.");
                            return false;
                        }

                        int musteriId = Convert.ToInt32(result);

                        // Update the password in the database
                        string sqlUpdatePassword = "UPDATE Musteri_Bilgi SET Sifre = @temporaryPassword WHERE MusteriId = @musteriId";

                        using (NpgsqlCommand commandUpdatePassword = new NpgsqlCommand(sqlUpdatePassword, connection))
                        {
                            commandUpdatePassword.Parameters.AddWithValue("@temporaryPassword", temporaryPassword);
                            commandUpdatePassword.Parameters.AddWithValue("@musteriId", musteriId);

                            int rowsAffected = commandUpdatePassword.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine($"\nGeçici şifreniz başarıyla oluşturuldu. Şifreniz: {temporaryPassword}");
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("\nŞifre sıfırlama işlemi başarısız. Lütfen tekrar deneyin.");
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return false;
            }
        }

        public bool ChangePassword(int musteriId, int newPassword)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string sql = "UPDATE Musteri_Bilgi SET Sifre = @newPassword WHERE MusteriId = @musteriId";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@newPassword", newPassword);
                        command.Parameters.AddWithValue("@musteriId", musteriId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Şifreniz başarıyla değiştirildi.");
                        }
                        else
                        {
                            Console.WriteLine("Şifre değiştirme işlemi başarısız. Lütfen tekrar deneyin.");
                        }
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return false;
            }
        }

        /*
        ************************************************************************************************
        *****************************************TransferDB.cs******************************************
        ************************************************************************************************
        */

        public bool HesaplarArasiTransfer(long kaynakHesapNo, long hedefHesapNo, decimal miktar)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    // Kontrol et: Kaynak hesap var mı?
                    string sqlSelectKaynak = "SELECT Bakiye FROM public.Hesap WHERE HesapNo = @kaynakHesapNo";
                    string sqlSelect = "SELECT Bakiye FROM public.Hesap WHERE HesapNo = @hesapNo";

                    using (NpgsqlCommand commandSelect = new NpgsqlCommand(sqlSelect, connection))
                    {
                        commandSelect.Parameters.AddWithValue("@hesapNo", kaynakHesapNo);

                        decimal eskiBakiye = Convert.ToDecimal(commandSelect.ExecuteScalar());
                        decimal yeniBakiye = eskiBakiye - miktar;

                        using (NpgsqlCommand commandSelectKaynak = new NpgsqlCommand(sqlSelectKaynak, connection))
                        {
                            commandSelectKaynak.Parameters.AddWithValue("@kaynakHesapNo", kaynakHesapNo);

                            object result = commandSelectKaynak.ExecuteScalar();
                            if (result == null)
                            {

                                // Log the failed transfer
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    AccountNumber = kaynakHesapNo,
                                    TargetAccountNumber = hedefHesapNo,
                                    TransactionType = EnumTransactionType.BOATransfer,
                                    TransactionStatus = EnumTransactionStatus.Failed,
                                    Amount = miktar,
                                    OldBalance = eskiBakiye,
                                    NewBalance = yeniBakiye,
                                    Timestamp = DateTime.Now
                                };

                                LogTransaction(transactionLog);

                                Console.WriteLine("\nKaynak hesap bulunamadı.");
                                return false;
                            }

                            decimal kaynakBakiye = (decimal)result;
                            if (kaynakBakiye < miktar)
                            {

                                // Log the failed transfer
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    AccountNumber = kaynakHesapNo,
                                    TargetAccountNumber = hedefHesapNo,
                                    TransactionType = EnumTransactionType.BOATransfer,
                                    TransactionStatus = EnumTransactionStatus.Failed,
                                    Amount = miktar,
                                    OldBalance = eskiBakiye,
                                    NewBalance = eskiBakiye,
                                    Timestamp = DateTime.Now
                                };

                                LogTransaction(transactionLog);

                                Console.WriteLine("\nYetersiz bakiye. Transfer gerçekleştirilemedi.");
                                return false;
                            }
                        }

                        // Kontrol et: Hedef hesap var mı?
                        string sqlSelectHedef = "SELECT Bakiye FROM public.Hesap WHERE HesapNo = @hedefHesapNo";
                        using (NpgsqlCommand commandSelectHedef = new NpgsqlCommand(sqlSelectHedef, connection))
                        {
                            commandSelectHedef.Parameters.AddWithValue("@hedefHesapNo", hedefHesapNo);

                            object result = commandSelectHedef.ExecuteScalar();
                            if (result == null)
                            {

                                // Log the failed transfer
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    AccountNumber = kaynakHesapNo,
                                    TargetAccountNumber = hedefHesapNo,
                                    TransactionType = EnumTransactionType.BOATransfer,
                                    TransactionStatus = EnumTransactionStatus.Failed,
                                    Amount = miktar,
                                    OldBalance = eskiBakiye,
                                    NewBalance = eskiBakiye,
                                    Timestamp = DateTime.Now
                                };

                                LogTransaction(transactionLog);

                                Console.WriteLine("\nHedef hesap bulunamadı.");
                                return false;
                            }
                        }

                        // Para transferi gerçekleştir
                        string sqlUpdateKaynak = "UPDATE public.Hesap SET Bakiye = Bakiye - @miktar WHERE HesapNo = @kaynakHesapNo";
                        string sqlUpdateHedef = "UPDATE public.Hesap SET Bakiye = Bakiye + @miktar WHERE HesapNo = @hedefHesapNo";

                        using (NpgsqlTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                using (NpgsqlCommand commandUpdateKaynak = new NpgsqlCommand(sqlUpdateKaynak, connection, transaction))
                                {
                                    commandUpdateKaynak.Parameters.AddWithValue("@miktar", miktar);
                                    commandUpdateKaynak.Parameters.AddWithValue("@kaynakHesapNo", kaynakHesapNo);

                                    commandUpdateKaynak.ExecuteNonQuery();
                                }

                                using (NpgsqlCommand commandUpdateHedef = new NpgsqlCommand(sqlUpdateHedef, connection, transaction))
                                {
                                    commandUpdateHedef.Parameters.AddWithValue("@miktar", miktar);
                                    commandUpdateHedef.Parameters.AddWithValue("@hedefHesapNo", hedefHesapNo);

                                    commandUpdateHedef.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                Console.WriteLine("\nPara transferi başarıyla gerçekleştirildi.");

                                // Log the successful transfer
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    AccountNumber = kaynakHesapNo,
                                    TargetAccountNumber = hedefHesapNo,
                                    TransactionType = EnumTransactionType.BOATransfer,
                                    TransactionStatus = EnumTransactionStatus.Success,
                                    Amount = miktar,
                                    OldBalance = eskiBakiye,
                                    NewBalance = yeniBakiye,
                                    Timestamp = DateTime.Now
                                };

                                LogTransaction(transactionLog);

                                return true;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"\nHata oluştu: {ex.Message}");
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return false;
            }
        }

        public bool Havale(long kaynakHesapNo, long hedefHesapNo, decimal miktar)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    decimal transactionFee = GetTransactionFee(EnumTransactionFeeType.Havale);

                    // Check if the hedefHesapNo exists in the database
                    string sqlSelectKaynak = "SELECT HesapNo FROM public.Hesap WHERE HesapNo = @hedefHesapNo";

                    string sqlSelect = "SELECT Bakiye FROM public.Hesap WHERE HesapNo = @hesapNo";

                    using (NpgsqlCommand commandSelectKaynak = new NpgsqlCommand(sqlSelectKaynak, connection))
                    {
                        commandSelectKaynak.Parameters.AddWithValue("@hedefHesapNo", hedefHesapNo);

                        object result = commandSelectKaynak.ExecuteScalar();
                        if (result == null)
                        {
                            Console.WriteLine("Hedef hesap numarası bankamıza ait değil lütfen EFT işlemi gerçekleştiriniz.");
                        }

                        else
                        {
                            using (NpgsqlCommand commandSelect = new NpgsqlCommand(sqlSelect, connection))
                            {


                                commandSelect.Parameters.AddWithValue("@hesapNo", kaynakHesapNo);

                                decimal eskiBakiye = Convert.ToDecimal(commandSelect.ExecuteScalar());
                                decimal yeniBakiye = eskiBakiye - miktar - transactionFee;

                                // Check if kaynakBakiye is sufficient for the transfer
                                if (yeniBakiye >= 0)
                                {
                                    // Para transferi gerçekleştir
                                    string sqlUpdateKaynak = "UPDATE public.Hesap SET Bakiye = Bakiye - @miktar - @transactionFee WHERE HesapNo = @kaynakHesapNo";
                                    string sqlUpdateHedef = "UPDATE public.Hesap SET Bakiye = Bakiye + @miktar WHERE HesapNo = @hedefHesapNo";

                                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                                    {
                                        try
                                        {
                                            using (NpgsqlCommand commandUpdateKaynak = new NpgsqlCommand(sqlUpdateKaynak, connection, transaction))
                                            {
                                                commandUpdateKaynak.Parameters.AddWithValue("@miktar", miktar);
                                                commandUpdateKaynak.Parameters.AddWithValue("@transactionFee", transactionFee);
                                                commandUpdateKaynak.Parameters.AddWithValue("@kaynakHesapNo", kaynakHesapNo);

                                                commandUpdateKaynak.ExecuteNonQuery();
                                            }

                                            using (NpgsqlCommand commandUpdateHedef = new NpgsqlCommand(sqlUpdateHedef, connection, transaction))
                                            {
                                                commandUpdateHedef.Parameters.AddWithValue("@miktar", miktar);
                                                commandUpdateHedef.Parameters.AddWithValue("@hedefHesapNo", hedefHesapNo);

                                                commandUpdateHedef.ExecuteNonQuery();
                                            }

                                            transaction.Commit();
                                            Console.WriteLine("\nHavale işlemi başarıyla gerçekleştirildi.");

                                            // Log the successful transfer
                                            DTOTransactionLog transactionLog = new DTOTransactionLog
                                            {
                                                AccountNumber = kaynakHesapNo,
                                                TargetAccountNumber = hedefHesapNo,
                                                TransactionType = EnumTransactionType.Havale,
                                                TransactionStatus = EnumTransactionStatus.Success,
                                                Amount = miktar,
                                                OldBalance = eskiBakiye,
                                                NewBalance = yeniBakiye,
                                                TransactionFee = transactionFee,
                                                Timestamp = DateTime.Now
                                            };

                                            LogTransaction(transactionLog);

                                            return true;
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"\nHata oluştu: {ex.Message}");
                                            transaction.Rollback();

                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    // Not enough balance
                                    Console.WriteLine("\nYetersiz bakiye. Transfer gerçekleştirilemedi.");

                                    // Log the failed transfer
                                    DTOTransactionLog transactionLog = new DTOTransactionLog
                                    {
                                        AccountNumber = kaynakHesapNo,
                                        TargetAccountNumber = hedefHesapNo,
                                        TransactionType = EnumTransactionType.Havale,
                                        TransactionStatus = EnumTransactionStatus.Failed,
                                        Amount = miktar,
                                        OldBalance = eskiBakiye,
                                        NewBalance = eskiBakiye,
                                        Timestamp = DateTime.Now
                                    };

                                    LogTransaction(transactionLog);
                                }
                            }
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return false;
            }
        }

        public bool EFT(long kaynakHesapNo, long hedefHesapNo, decimal miktar)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    decimal transactionFee = GetTransactionFee(EnumTransactionFeeType.EFT);

                    // Check if the hedefHesapNo exists in the database
                    string sqlSelectKaynak = "SELECT HesapNo FROM public.Hesap WHERE HesapNo = @hedefHesapNo";

                    // Kontrol et: Kaynak hesap var mı?
                    string sqlSelect = "SELECT Bakiye FROM public.Hesap WHERE HesapNo = @hesapNo";

                    using (NpgsqlCommand commandSelectKaynak = new NpgsqlCommand(sqlSelectKaynak, connection))
                    {
                        commandSelectKaynak.Parameters.AddWithValue("@hedefHesapNo", hedefHesapNo);

                        object result = commandSelectKaynak.ExecuteScalar();
                        if (result == null)
                        {
                            using (NpgsqlCommand commandSelect = new NpgsqlCommand(sqlSelect, connection))
                            {
                                commandSelect.Parameters.AddWithValue("@hesapNo", kaynakHesapNo);

                                decimal eskiBakiye = Convert.ToDecimal(commandSelect.ExecuteScalar());
                                decimal yeniBakiye = eskiBakiye - miktar - transactionFee;

                                // Check if kaynakBakiye is sufficient for the EFT
                                if (yeniBakiye >= 0)
                                {
                                    // Para transferi gerçekleştir
                                    string sqlUpdateKaynak = "UPDATE public.Hesap SET Bakiye = Bakiye - @miktar - @transactionFee WHERE HesapNo = @kaynakHesapNo";

                                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                                    {
                                        try
                                        {
                                            using (NpgsqlCommand commandUpdateKaynak = new NpgsqlCommand(sqlUpdateKaynak, connection, transaction))
                                            {
                                                commandUpdateKaynak.Parameters.AddWithValue("@miktar", miktar);
                                                commandUpdateKaynak.Parameters.AddWithValue("@transactionFee", transactionFee);
                                                commandUpdateKaynak.Parameters.AddWithValue("@kaynakHesapNo", kaynakHesapNo);

                                                commandUpdateKaynak.ExecuteNonQuery();
                                            }

                                            transaction.Commit();
                                            Console.WriteLine("\nEFT işlemi başarıyla gerçekleştirildi.");

                                            // Log the successful EFT
                                            DTOTransactionLog transactionLog = new DTOTransactionLog
                                            {
                                                AccountNumber = kaynakHesapNo,
                                                TargetAccountNumber = hedefHesapNo,
                                                TransactionType = EnumTransactionType.EFT,
                                                TransactionStatus = EnumTransactionStatus.Success,
                                                Amount = miktar,
                                                OldBalance = eskiBakiye,
                                                NewBalance = yeniBakiye,
                                                TransactionFee = transactionFee,
                                                Timestamp = DateTime.Now
                                            };

                                            LogTransaction(transactionLog);

                                            return true;
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"\nHata oluştu: {ex.Message}");
                                            transaction.Rollback();

                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    // Not enough balance
                                    Console.WriteLine("\nYetersiz bakiye. EFT işlemi gerçekleştirilemedi.");

                                    // Log the failed EFT
                                    DTOTransactionLog transactionLog = new DTOTransactionLog
                                    {
                                        AccountNumber = kaynakHesapNo,
                                        TargetAccountNumber = hedefHesapNo,
                                        TransactionType = EnumTransactionType.EFT,
                                        TransactionStatus = EnumTransactionStatus.Failed,
                                        Amount = miktar,
                                        OldBalance = eskiBakiye,
                                        NewBalance = eskiBakiye,
                                        Timestamp = DateTime.Now
                                    };

                                    LogTransaction(transactionLog);
                                }
                            }
                        }

                        else
                        {
                            Console.WriteLine("Hedef hesap numarası bankamıza ait lütfen Havale işlemi gerçekleştiriniz.");
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
                return false;
            }
        }

        /*
        ************************************************************************************************
        ***************************************TransactionLogDB.cs****************************************
        ************************************************************************************************
        */

        public void LogTransaction(DTOTransactionLog transactionLog)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string sql = "INSERT INTO Transaction_Log (AccountNumber, TargetAccountNumber ,TransactionType, TransactionStatus, Amount ,OldBalance, NewBalance, TransactionFee, Timestamp) " +
                        "VALUES (@accountNumber, @targetAccountNumber ,@transactionType, @transactionStatus, @amount ,@oldBalance, @newBalance, @transactionFee, @timestamp)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@accountNumber", transactionLog.AccountNumber);
                        command.Parameters.AddWithValue("@targetAccountNumber", transactionLog.TargetAccountNumber);
                        command.Parameters.AddWithValue("@transactionType", (int)transactionLog.TransactionType);
                        command.Parameters.AddWithValue("@transactionStatus", (int)transactionLog.TransactionStatus);
                        command.Parameters.AddWithValue("@amount", transactionLog.Amount);
                        command.Parameters.AddWithValue("@oldBalance", transactionLog.OldBalance);
                        command.Parameters.AddWithValue("@newBalance", transactionLog.NewBalance);
                        command.Parameters.AddWithValue("@transactionFee", transactionLog.TransactionFee);
                        command.Parameters.AddWithValue("@timestamp", transactionLog.Timestamp);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /*
        ************************************************************************************************
        **************************************TransactionFeeDB.cs***************************************
        ************************************************************************************************
        */

        public decimal GetTransactionFee(EnumTransactionFeeType transactionType)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string sql = "SELECT FeeAmount FROM Transaction_Fee WHERE TransactionType = @transactionType";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        // Convert the EnumTransactionFeeType to its integer representation
                        int transactionTypeValue = (int)transactionType;
                        command.Parameters.AddWithValue("@transactionType", transactionTypeValue);

                        object result = command.ExecuteScalar();

                        if (result != null && decimal.TryParse(result.ToString(), out decimal feeAmount))
                        {
                            return feeAmount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error and handle it
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }

            return 0.00m;
        }
    }
}