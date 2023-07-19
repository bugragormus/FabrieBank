using System;
using Microsoft.Data.SqlClient;
using System.Data.SQLite;
using FabrieBank.DAL;

namespace FabrieBank.Entity
{
    public class TransferDB
    {
        private DataAccessLayer dataAccessLayer;

        public TransferDB()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public bool HesaplarArasiTransfer(long kaynakHesapNo, long hedefHesapNo, long miktar)
        {
            return dataAccessLayer.HesaplarArasiTransfer(kaynakHesapNo, hedefHesapNo, miktar);
        }

        public bool HavaleEFT(long kaynakHesapNo, long hedefHesapNo, long miktar)
        {
            return dataAccessLayer.HavaleEFT(kaynakHesapNo, hedefHesapNo, miktar);
        }
    }
}