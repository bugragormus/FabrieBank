using FabrieBank.DAL.Common.Enums;
using Npgsql;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

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
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
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
        /// TransferDB.cs
        /// </summary>
        /// <param name="kaynakHesapNo"></param>
        /// <param name="hedefHesapNo"></param>
        /// <param name="miktar"></param>
        /// <returns></returns>
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
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
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
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
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
                        command.Parameters.AddWithValue("@timestamp", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
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

                    string functionName = "func_readtransactionfee";

                    string sql = $"SELECT * FROM {functionName}(@transactionType)";

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
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }

            return 0.00m;
        }
    }
}