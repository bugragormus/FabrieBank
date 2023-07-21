using FabrieBank.Common;
using FabrieBank.BLL;
namespace FabrieBank
{
    public class AccountInfo
    {
        private AccountLogic accountLogic;

        public AccountInfo()
        {
            accountLogic = new AccountLogic();
        }

        public void AccountInfoM(DTOCustomer customer)
        {
            Console.Clear();
            Console.WriteLine("******************************************************");
            Console.WriteLine("*            |                         |             *");
            Console.WriteLine("*            |     Hesap Bilgileri     |             *");
            Console.WriteLine("*            V                         V             *");
            Console.WriteLine("******************************************************\n");

            accountLogic.AccountLogicM(customer);
        }
    }
}