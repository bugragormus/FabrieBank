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

    }
}
