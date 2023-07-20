using Microsoft.Data.SqlClient;
using FabrieBank.Common;
using FabrieBank.Common.Enums;
using System.Reflection;
using System.Data;
using Npgsql;

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

                string functionName = "usp_InsertErrorLog";

                string sqlQuery = $"CALL {functionName}(@errorDateTime, @errorMessage, @stackTrace, @operationName)";

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

                    string sql = "SELECT * FROM Hesap WHERE MusteriId = @musteriId";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@musteriId", musteriId);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DTOAccountInfo dTOAccountInfo = new DTOAccountInfo
                                {
                                    HesapNo = reader.GetInt64(0),
                                    Bakiye = reader.GetInt64(1),
                                    MusteriId = reader.GetInt32(2),
                                    DovizCins = (EnumDovizCinsleri.DovizCinsleri)reader.GetInt32(3),
                                    HesapAdi = reader.GetString(4),
                                };

                                accountInfos.Add(dTOAccountInfo);
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

                        long bakiye = Convert.ToInt64(result);
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

        public void Deposit(long hesapNo, long bakiye)
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

                        long eskiBakiye = Convert.ToInt64(commandSelect.ExecuteScalar());
                        long yeniBakiye = eskiBakiye + bakiye;

                        using (NpgsqlCommand commandUpdate = new NpgsqlCommand(sqlUpdate, connection))
                        {
                            commandUpdate.Parameters.AddWithValue("@bakiye", bakiye);
                            commandUpdate.Parameters.AddWithValue("@hesapNo", hesapNo);

                            int rowsAffected = commandUpdate.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("\nPara yatırma işlemi başarılı.");
                                Console.WriteLine($"Eski bakiye: {eskiBakiye}");
                                Console.WriteLine($"Yeni bakiye: {yeniBakiye}");
                            }
                            else
                            {
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

        public void Withdraw(long hesapNo, long bakiye)
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

                        long eskiBakiye = Convert.ToInt64(commandSelect.ExecuteScalar());
                        long yeniBakiye = eskiBakiye - bakiye;

                        if (yeniBakiye >= 0)
                        {
                            using (NpgsqlCommand commandUpdate = new NpgsqlCommand(sqlUpdate, connection))
                            {
                                commandUpdate.Parameters.AddWithValue("@bakiye", bakiye);
                                commandUpdate.Parameters.AddWithValue("@hesapNo", hesapNo);

                                int rowsAffected = commandUpdate.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("\nPara çekme işlemi başarılı.");
                                    Console.WriteLine($"Eski bakiye: {eskiBakiye}");
                                    Console.WriteLine($"Yeni bakiye: {yeniBakiye}");
                                }
                                else
                                {
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
                    long hesapNo = long.Parse(hesapNumarasi); // or Convert.ToInt64(hesapNumarasi)

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

        /*
        ************************************************************************************************
        *****************************************TransferDB.cs******************************************
        ************************************************************************************************
        */

        public bool HesaplarArasiTransfer(long kaynakHesapNo, long hedefHesapNo, long miktar)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    // Kontrol et: Kaynak hesap var mı?
                    string sqlSelectKaynak = "SELECT Bakiye FROM public.Hesap WHERE HesapNo = @kaynakHesapNo";
                    using (NpgsqlCommand commandSelectKaynak = new NpgsqlCommand(sqlSelectKaynak, connection))
                    {
                        commandSelectKaynak.Parameters.AddWithValue("@kaynakHesapNo", kaynakHesapNo);

                        object result = commandSelectKaynak.ExecuteScalar();
                        if (result == null)
                        {
                            Console.WriteLine("\nKaynak hesap bulunamadı.");
                            return false;
                        }

                        long kaynakBakiye = (long)result;
                        if (kaynakBakiye < miktar)
                        {
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

        public bool HavaleEFT(long kaynakHesapNo, long hedefHesapNo, long miktar)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    // Kontrol et: Kaynak hesap var mı?
                    string sqlSelectKaynak = "SELECT Bakiye FROM public.Hesap WHERE HesapNo = @kaynakHesapNo";
                    using (NpgsqlCommand commandSelectKaynak = new NpgsqlCommand(sqlSelectKaynak, connection))
                    {
                        commandSelectKaynak.Parameters.AddWithValue("@kaynakHesapNo", kaynakHesapNo);

                        object result = commandSelectKaynak.ExecuteScalar();
                        if (result == null)
                        {
                            Console.WriteLine("\nKaynak hesap bulunamadı.");
                            return false;
                        }

                        long kaynakBakiye = (long)result;
                        if (kaynakBakiye < miktar + 5) // 5 birim ek para kesintisi
                        {
                            Console.WriteLine("\nYetersiz bakiye. Transfer gerçekleştirilemedi.");
                            return false;
                        }
                    }

                    // Para transferi gerçekleştir
                    string sqlUpdateKaynak = "UPDATE public.Hesap SET Bakiye = Bakiye - @miktar - 5 WHERE HesapNo = @kaynakHesapNo";
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
                            Console.WriteLine("\nHavale/EFT işlemi başarıyla gerçekleştirildi.");
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
    }
}
