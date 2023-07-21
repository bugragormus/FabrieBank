using FabrieBank.Common.Enums;
using FabrieBank.DAL;
using FabrieBank.Services;

namespace FabrieBank.Entity
{
    public class ATM
    {
        private readonly CurrencyService _currencyService;
        private DataAccessLayer dataAccessLayer;

        public ATM()
        {
            dataAccessLayer = new DataAccessLayer();
            _currencyService = new CurrencyService();
        }

        public void ParaYatirma(long hesapNo, decimal bakiye)
        {
            dataAccessLayer.Deposit(hesapNo, bakiye);
        }

        public void ParaCekme(long hesapNo, decimal bakiye)
        {
            dataAccessLayer.Withdraw(hesapNo, bakiye);
        }
    }
}