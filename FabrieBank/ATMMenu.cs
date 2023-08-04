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

        public void ShowMenu()
        {
            string choice;

            do
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine("ATM TRANSACTIONS");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Deposit");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. Back to Main Menu");
                Console.WriteLine("==============================");
                Console.Write("Make your choice (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Deposit();
                        break;
                    case "2":
                        Withdraw();
                        break;
                    case "3":
                        Console.WriteLine("Checked out from ATM.");
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Try again.");
                        break;
                }
            } while (choice != "3");
        }

        private void Deposit()
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
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }
        }

        private void Withdraw()
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
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }
        }
    }
}