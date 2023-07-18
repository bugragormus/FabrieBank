using FabrieBank.Common;
using System.Collections.Generic;
using FabrieBank.DAL;
namespace FabrieBank.Entity
{
    public class AccInfoDB
    {
        private DataAccessLayer dataAccessLayer;

        public AccInfoDB()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public List<DTOAccountInfo> AccInfo(int musteriId)
        {
            return dataAccessLayer.GetAccountInfo(musteriId);
        }

        public bool HesapSil(long hesapNo)
        {
            return dataAccessLayer.DeleteAccount(hesapNo);
        }
    }
}