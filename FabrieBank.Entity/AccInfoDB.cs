using System;
using Microsoft.Data.SqlClient;
using FabrieBank.Common;
using FabrieBank.Common.Enums;

namespace FabrieBank.Entity
{
    public class AccInfoDB
    {
        private SqlConnectionStringBuilder database1;
        private Database database;

        public AccInfoDB()
        {
            database = new Database();
            database1 = database.CallDB();
        }

        public List<DTOAccountInfo> AccInfo(int musteriId)
        {
            List<DTOAccountInfo> accountInfos = new List<DTOAccountInfo>();

            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM dbo.Hesap WHERE MusteriId = @musteriId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@musteriId", musteriId);

                    using (SqlDataReader reader = command.ExecuteReader())
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

            return accountInfos;
        }

        public bool HesapSil(long hesapNo)
        {
            using (SqlConnection connection = new SqlConnection(database1.ConnectionString))
            {
                connection.Open();

                // Hesap bakiyesini kontrol et
                string sqlSelectBakiye = "SELECT Bakiye FROM dbo.Hesap WHERE HesapNo = @hesapNo";
                using (SqlCommand commandSelectBakiye = new SqlCommand(sqlSelectBakiye, connection))
                {
                    commandSelectBakiye.Parameters.AddWithValue("@hesapNo", hesapNo);

                    object result = commandSelectBakiye.ExecuteScalar();
                    if (result == null)
                    {
                        Console.WriteLine("\nHesap bulunamadı.");
                        return false;
                    }

                    long bakiye = (long)result;
                    if (bakiye != 0)
                    {
                        Console.WriteLine("\nHesap bakiyesi 0 değil. Lütfen bakiyeyi başka bir hesaba aktarın.");
                        return false;
                    }
                }

                // Hesabı sil
                string sqlDeleteHesap = "DELETE FROM dbo.Hesap WHERE HesapNo = @hesapNo";
                using (SqlCommand commandDeleteHesap = new SqlCommand(sqlDeleteHesap, connection))
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



    }
}
