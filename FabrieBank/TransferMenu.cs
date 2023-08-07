using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;
using FabrieBank.BLL.Logic;

namespace FabrieBank
{
    public class TransferMenu
    {
        private int customerId;
        private BTransaction transactionLogic;
        private EAccountInfo eAccount;

        public TransferMenu(int customerId)
        {
            this.customerId = customerId;
            transactionLogic = new BTransaction();
            eAccount = new EAccountInfo();
        }

        public void ShowMenu()
        {
            DTOAccountInfo dTOAccount = new DTOAccountInfo()
            {
                CustomerId = customerId
            };
            List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccount);

            string choice;
            do
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine("MONEY TRANSFERS");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Transfer Between Accounts");
                Console.WriteLine("2. To Another Account Havale/EFT");
                Console.WriteLine("3. Back to Main Menu");
                Console.WriteLine("==============================");
                Console.Write("Make your choice (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        TransferBetweenAccounts(accountInfos);
                        break;
                    case "2":
                        HavaleEFT(accountInfos);
                        break;
                    case "3":
                        Console.WriteLine("Exited from money transfers");
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Try again.");
                        break;
                }
            } while (choice != "3");
        }

        private void HavaleEFT(List<DTOAccountInfo> accountInfos)
        {
            Console.WriteLine("\nWhich account do you want to withdraw money from?");
            transactionLogic.PrintAccountList(accountInfos);

            Console.Write("Source Account Index: ");
            int sourceAccountIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nEnter the account number you want to transfer money to: ");
            Console.Write("Target Account Number: ");
            long targetAccountNo = long.Parse(Console.ReadLine());

            Console.WriteLine("\nEnter the amount you want to transfer: ");
            decimal transferAmount = decimal.Parse(Console.ReadLine());

            DTOTransfer transfer = new DTOTransfer()
            {
                SourceAccountIndex = sourceAccountIndex,
                TargetAccountNo = targetAccountNo,
                Amount = transferAmount
            };

            transactionLogic.HavaleEFT(customerId, transfer);
        }

        private void TransferBetweenAccounts(List<DTOAccountInfo> accountInfos)
        {
            Console.WriteLine("\nWhich account do you want to withdraw money from?");
            transactionLogic.PrintAccountList(accountInfos);

            Console.Write("Source Account Index: ");
            int sourceAccountIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nWhich account do you want to transfer money to?");
            transactionLogic.PrintAccountList(accountInfos);

            Console.Write("Target Account Index: ");
            int targetAccountIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nEnter the amount you want to transfer: ");
            decimal transferAmount = decimal.Parse(Console.ReadLine());

            DTOTransfer transfer = new DTOTransfer()
            {
                SourceAccountIndex = sourceAccountIndex,
                TargetAccountIndex = targetAccountIndex,
                Amount = transferAmount
            };

            transactionLogic.TransferBetweenAccounts(customerId, transfer);
        }
    }
}