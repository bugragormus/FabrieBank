using FabrieBank.DAL;
using Npgsql;

namespace FabrieBank.DAL.Entity
{
    public class CreateAccountDB
    {
        private DataAccessLayer dataAccessLayer;

        public CreateAccountDB()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public void CreateAccount(int musteriId, int dovizCinsi, string hesapAdi)
        {
            dataAccessLayer.CreateAccount(musteriId, dovizCinsi, hesapAdi);
        }

        public string GetAndIncrementHesapNumarasi(int dovizCinsi, NpgsqlConnection connection)
        {
            return dataAccessLayer.GetAndIncrementHesapNumarasi(dovizCinsi, connection);
        }
    }
}