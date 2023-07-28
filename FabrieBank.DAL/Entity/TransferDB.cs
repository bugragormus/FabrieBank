using FabrieBank.DAL;

namespace FabrieBank.DAL.Entity
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

        public bool Havale(long kaynakHesapNo, long hedefHesapNo, decimal miktar)
        {
            return dataAccessLayer.Havale(kaynakHesapNo, hedefHesapNo, miktar);
        }

        public bool EFT(long kaynakHesapNo, long hedefHesapNo, decimal miktar)
        {
            return dataAccessLayer.EFT(kaynakHesapNo, hedefHesapNo, miktar);
        }
    }
}