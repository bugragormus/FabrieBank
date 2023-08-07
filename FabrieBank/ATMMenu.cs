using FabrieBank.DAL.Entity;
using FabrieBank.BLL.Logic;
using FabrieBank.DAL.Common.DTOs;

namespace FabrieBank
{
    public class ATMMenu
    {
        private BAccount atm;

        public ATMMenu()
        {
            atm = new BAccount();
        }

        public void Deposit()
        {
            try
            {
                Console.WriteLine("\nAccount Number: ");
                Console.Write(">>> ");
                string accountNoInput = Console.ReadLine();
                long accountNo;

                if (long.TryParse(accountNoInput, out accountNo))
                {
                    Console.WriteLine("\nAmount: ");
                    Console.Write(">>> ");
                    string amountInput = Console.ReadLine();
                    decimal amount;

                    if (decimal.TryParse(amountInput, out amount))
                    {
                        DTOAccountInfo accountInfo = new DTOAccountInfo()
                        {
                            AccountNo = accountNo
                        };

                        atm.Deposit(accountInfo, amount);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input! Please enter a valid amount.");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Please enter a valid account number.");
                }

            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

        public void Withdraw()
        {
            try
            {
                Console.WriteLine("\nAccount Number: ");
                Console.Write(">>> ");
                string accountNoInput = Console.ReadLine();
                long accountNo;

                if (long.TryParse(accountNoInput, out accountNo))
                {
                    Console.WriteLine("\nAmount: ");
                    Console.Write(">>> ");
                    string amountInput = Console.ReadLine();
                    decimal amount;

                    if (decimal.TryParse(amountInput, out amount))
                    {
                        DTOAccountInfo accountInfo = new DTOAccountInfo()
                        {
                            AccountNo = accountNo,
                            Balance = amount
                        };

                        atm.Withdraw(accountInfo, amount);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input! Please enter a valid amount.");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Please enter a valid account number.");
                }
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }
    }
}