using FabrieBank.DAL.Common.Enums;
using System.Reflection;
using Npgsql;
using FabrieBank.DAL.Common.DTOs;

namespace FabrieBank.DAL
{
    public class DataAccessLayer
    {
        private NpgsqlConnectionStringBuilder database;

        public DataAccessLayer()
        {
            database = CallDB();
        }

        /// <summary>
        /// DB Bağlanma
        /// </summary>
        /// <returns></returns>
        public NpgsqlConnectionStringBuilder CallDB()
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

        /// <summary>
        /// ErrorLoggerDB.cs
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="methodName"></param>
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

        /// <summary>
        /// LogInDB.cs
        /// </summary>
        /// <param name="musteriId"></param>
        /// <param name="telNo"></param>
        /// <param name="email"></param>
        /// <returns></returns>

        //public bool IsCredentialsValid(long tckn, int sifre)
        //{
        //    try
        //    {
        //        using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
        //        {
        //            connection.Open();

        //            string sql = "SELECT COUNT(*) FROM public.Musteri_Bilgi WHERE Tckn = @tckn AND Sifre = @sifre";

        //            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
        //            {
        //                command.Parameters.AddWithValue("@tckn", tckn);
        //                command.Parameters.AddWithValue("@sifre", sifre);

        //                int result = Convert.ToInt32(command.ExecuteScalar());

        //                return result > 0;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error to the database using the ErrorLoggerDB
        //        MethodBase method = MethodBase.GetCurrentMethod();
        //        LogError(ex, method.ToString());

        //        // Handle the error (display a user-friendly message, rollback transactions, etc.)
        //        Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
        //        return IsCredentialsValid(tckn, sifre);
        //    }
        //}

        //public bool ForgotPassword(long tckn, string email, int temporaryPassword)
        //{
        //    try
        //    {
        //        using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
        //        {
        //            connection.Open();

        //            // Check if the user with the given TCKN and email exists
        //            string sqlSelect = "SELECT MusteriId FROM Musteri_Bilgi WHERE Tckn = @tckn AND Email = @email";

        //            using (NpgsqlCommand commandSelect = new NpgsqlCommand(sqlSelect, connection))
        //            {
        //                commandSelect.Parameters.AddWithValue("@tckn", tckn);
        //                commandSelect.Parameters.AddWithValue("@email", email);

        //                object result = commandSelect.ExecuteScalar();

        //                if (result == null)
        //                {
        //                    Console.WriteLine("\nHatalı TCKN veya e-posta adresi. Şifre sıfırlama işlemi başarısız.");
        //                    return false;
        //                }

        //                int musteriId = Convert.ToInt32(result);

        //                // Update the password in the database
        //                string sqlUpdatePassword = "UPDATE Musteri_Bilgi SET Sifre = @temporaryPassword WHERE MusteriId = @musteriId";

        //                using (NpgsqlCommand commandUpdatePassword = new NpgsqlCommand(sqlUpdatePassword, connection))
        //                {
        //                    commandUpdatePassword.Parameters.AddWithValue("@temporaryPassword", temporaryPassword);
        //                    commandUpdatePassword.Parameters.AddWithValue("@musteriId", musteriId);

        //                    int rowsAffected = commandUpdatePassword.ExecuteNonQuery();

        //                    if (rowsAffected > 0)
        //                    {
        //                        Console.WriteLine($"\nGeçici şifreniz başarıyla oluşturuldu. Şifreniz: {temporaryPassword}");
        //                        return true;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine("\nŞifre sıfırlama işlemi başarısız. Lütfen tekrar deneyin.");
        //                        return false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error to the database using the ErrorLoggerDB
        //        MethodBase method = MethodBase.GetCurrentMethod();
        //        LogError(ex, method.ToString());

        //        // Handle the error (display a user-friendly message, rollback transactions, etc.)
        //        Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
        //        return false;
        //    }
        //}

        /// <summary>
        /// TransferDB.cs
        /// </summary>
        /// <param name="kaynakHesapNo"></param>
        /// <param name="hedefHesapNo"></param>
        /// <param name="miktar"></param>
        /// <returns></returns>
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

        /// <summary>
        /// TransactionLogDB.cs
        /// </summary>
        /// <param name="transactionLog"></param>
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

        /// <summary>
        /// TransactionFeeDB.cs
        /// </summary>
        /// <param name="transactionType"></param>
        /// <returns></returns>
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