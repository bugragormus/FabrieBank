using System;
using Microsoft.Data.SqlClient;

namespace FabrieBank.Entity
{
    public class TransferDB
    {
        private SqlConnectionStringBuilder database1;
        private Database database;

        public TransferDB()
        {
            database = new Database();
            database1 = database.CallDB();
        }

        public bool HesaplarArasiTransfer(long kaynakHesapNo, long hedefHesapNo, long miktar)
        {
            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                connection.Open();

                // Kontrol et: Kaynak hesap var mı?
                string sqlSelectKaynak = "SELECT Bakiye FROM dbo.Hesap WHERE HesapNo = @kaynakHesapNo";
                using (SqlCommand commandSelectKaynak = new SqlCommand(sqlSelectKaynak, connection))
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
                string sqlSelectHedef = "SELECT Bakiye FROM dbo.Hesap WHERE HesapNo = @hedefHesapNo";
                using (SqlCommand commandSelectHedef = new SqlCommand(sqlSelectHedef, connection))
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
                string sqlUpdateKaynak = "UPDATE dbo.Hesap SET Bakiye = Bakiye - @miktar WHERE HesapNo = @kaynakHesapNo";
                string sqlUpdateHedef = "UPDATE dbo.Hesap SET Bakiye = Bakiye + @miktar WHERE HesapNo = @hedefHesapNo";

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand commandUpdateKaynak = new SqlCommand(sqlUpdateKaynak, connection, transaction))
                        {
                            commandUpdateKaynak.Parameters.AddWithValue("@miktar", miktar);
                            commandUpdateKaynak.Parameters.AddWithValue("@kaynakHesapNo", kaynakHesapNo);

                            commandUpdateKaynak.ExecuteNonQuery();
                        }

                        using (SqlCommand commandUpdateHedef = new SqlCommand(sqlUpdateHedef, connection, transaction))
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

        public bool HavaleEFT(long kaynakHesapNo, long hedefHesapNo, long miktar)
        {
            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                connection.Open();

                // Kontrol et: Kaynak hesap var mı?
                string sqlSelectKaynak = "SELECT Bakiye FROM dbo.Hesap WHERE HesapNo = @kaynakHesapNo";
                using (SqlCommand commandSelectKaynak = new SqlCommand(sqlSelectKaynak, connection))
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
                string sqlUpdateKaynak = "UPDATE dbo.Hesap SET Bakiye = Bakiye - @miktar - 5 WHERE HesapNo = @kaynakHesapNo";
                string sqlUpdateHedef = "UPDATE dbo.Hesap SET Bakiye = Bakiye + @miktar WHERE HesapNo = @hedefHesapNo";

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand commandUpdateKaynak = new SqlCommand(sqlUpdateKaynak, connection, transaction))
                        {
                            commandUpdateKaynak.Parameters.AddWithValue("@miktar", miktar);
                            commandUpdateKaynak.Parameters.AddWithValue("@kaynakHesapNo", kaynakHesapNo);

                            commandUpdateKaynak.ExecuteNonQuery();
                        }

                        using (SqlCommand commandUpdateHedef = new SqlCommand(sqlUpdateHedef, connection, transaction))
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

    }
}