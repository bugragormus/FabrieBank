using FabrieBank.DAL;
using FabrieBank.DAL.Common.DTOs;

namespace FabrieBank.DAL.Entity
{
    public class TransferDB
    {
        private DataAccessLayer dataAccessLayer;

        public TransferDB()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public bool EFT(long kaynakHesapNo, long hedefHesapNo, decimal miktar)
        {
            return dataAccessLayer.EFT(kaynakHesapNo, hedefHesapNo, miktar);
        }
    }
}