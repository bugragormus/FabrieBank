using FabrieBank.DAL.Common.DTOs;
using FabrieBank.BLL.Logic;

namespace FabrieBank
{
    public class AccountInfo
    {
        private BAccount accountLogic;

        public AccountInfo()
        {
            accountLogic = new BAccount();
        }

        public void AccountInfoM(DTOCustomer customer)
        {
            Console.Clear();
            Console.WriteLine("******************************************************");
            Console.WriteLine("*            |                         |             *");
            Console.WriteLine("*            |      Your Accounts      |             *");
            Console.WriteLine("*            V                         V             *");
            Console.WriteLine("******************************************************\n");

            accountLogic.AccountLogicM(customer);
        }
    }
}