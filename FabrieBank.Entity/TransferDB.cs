using System;
using Microsoft.Data.SqlClient;
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

        public bool HesaplarArasiTransfer(long kaynakHesapNo, long hedefHesapNo, decimal miktar)
        {
            return dataAccessLayer.HesaplarArasiTransfer(kaynakHesapNo, hedefHesapNo, miktar);
        }

        public bool HavaleEFT(long kaynakHesapNo, long hedefHesapNo, decimal miktar)
        {
            return dataAccessLayer.HavaleEFT(kaynakHesapNo, hedefHesapNo, miktar);
        }
    }
}