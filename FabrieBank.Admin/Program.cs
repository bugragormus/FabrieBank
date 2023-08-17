using System.Globalization;
using FabrieBank.Admin.BLL.Logic;
using FabrieBank.Admin.DAL.DTO;
using FabrieBank.Admin.PL;
using FabrieBank.DAL.Common.DTOs;

namespace FabrieBank
{
    class Program
    {
        /// <summary>
        /// Main method
        /// </summary>
        public static void Main()
        {
            string? ch;

            for (; ; )
            {
                Console.WriteLine("=======================");
                Console.WriteLine("Welcome To Admin System");
                Console.WriteLine("=======================");
                Console.WriteLine("1.Login          2.Exit");
                Console.WriteLine("=======================");
                Console.Write(">>> ");
                ch = Console.ReadLine();

                switch (ch)
                {
                    case "1":
                        PAdmin pAdmin = new PAdmin();
                        pAdmin.LogIn();
                        break;

                    case "2":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }

        /// <summary>
        /// Opening menu
        /// </summary>
        /// <param name="customer"></param>
        public void MainMenu(DTOAdmin admin)
        {
            string? ch;

            for (; ; )
            {
                Console.WriteLine("==============================================================================================================");
                Console.WriteLine("                                                MAIN MENU                                                     ");
                Console.WriteLine("==============================================================================================================");
                Console.WriteLine("1.Customer Operations    2.Account Operations     3.Logs      4.Fees     5.Monthly Transactions      6.Log Out");
                Console.WriteLine("==============================================================================================================");
                Console.Write(">>> ");
                ch = Console.ReadLine();
                Console.Clear();

                switch (ch)
                {
                    case "1":

                        CustomerMenu(admin);
                        break;

                    case "2":

                        PAdmin pAdmin = new PAdmin();
                        pAdmin.ListAccounts(admin);
                        break;

                    case "3":

                        LogMenu(admin);
                        break;

                    case "4":

                        FeeMenu(admin);
                        break;

                    case "5":

                        MonthlyTransactions(admin);
                        break;

                    case "6":

                        Main();
                        break;

                    default:

                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }

        /// <summary>
        /// Menu for customer operations
        /// </summary>
        /// <param name="customer"></param>
        public void CustomerMenu(DTOAdmin admin)
        {
            string choice;

            do
            {
                Console.WriteLine("==============================");
                Console.WriteLine("USER OPERATIONS");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Customer Status Update");
                Console.WriteLine("2. List Customers");
                Console.WriteLine("3. Back to Main Menu");
                Console.WriteLine("==============================");
                Console.Write("Make your pick (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PAdmin pAdmin = new PAdmin();
                        pAdmin.CustomerStatusUpdate();
                        break;
                    case "2":
                        PAdmin pAdmin1 = new PAdmin();
                        pAdmin1.ListCustomers(admin);
                        break;
                    case "3":
                        Console.WriteLine("\nExited from User Operations.\n");
                        break;
                    default:
                        Console.WriteLine("\nInvalid selection. Try again.\n");
                        break;
                }
            } while (choice != "3");
        }

        /// <summary>
        /// Menu for log operations
        /// </summary>
        /// <param name="admin"></param>
        public void LogMenu(DTOAdmin admin)
        {
            string choice;

            do
            {
                Console.WriteLine("==============================");
                Console.WriteLine("LOG OPERATIONS");
                Console.WriteLine("==============================");
                Console.WriteLine("1. List Error Logs");
                Console.WriteLine("2. List Transaction Logs");
                Console.WriteLine("3. Back to Main Menu");
                Console.WriteLine("==============================");
                Console.Write("Make your pick (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PAdmin pAdmin = new PAdmin();
                        pAdmin.ListErrorLogs(admin);
                        break;
                    case "2":
                        PAdmin pAdmin1 = new PAdmin();
                        pAdmin1.ListTransactionLogs(admin);
                        break;
                    case "3":
                        Console.WriteLine("\nExited from Log Operations.\n");
                        break;
                    default:
                        Console.WriteLine("\nInvalid selection. Try again.\n");
                        break;
                }
            } while (choice != "3");
        }

        /// <summary>
        /// Menu for fee operations
        /// </summary>
        /// <param name="admin"></param>
        public void FeeMenu(DTOAdmin admin)
        {
            string choice;

            do
            {
                Console.WriteLine("==============================");
                Console.WriteLine("FEE OPERATIONS");
                Console.WriteLine("==============================");
                Console.WriteLine("1. List Fee Table");
                Console.WriteLine("2. Add New Fee To Table");
                Console.WriteLine("3. Remove Fee From Table");
                Console.WriteLine("4. Update Fees");
                Console.WriteLine("5. Back to Main Menu");
                Console.WriteLine("==============================");
                Console.Write("Make your pick (1-5): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PAdmin pAdmin = new PAdmin();
                        pAdmin.ListFees(admin);
                        break;
                    case "2":
                        PAdmin pAdmin1 = new PAdmin();
                        pAdmin1.AddFee(admin);
                        break;
                    case "3":
                        PAdmin pAdmin2 = new PAdmin();
                        pAdmin2.RemoveFee(admin);
                        break;
                    case "4":
                        PAdmin pAdmin3 = new PAdmin();
                        pAdmin3.UpdateFee(admin);
                        break;
                    case "5":
                        Console.WriteLine("\nExited from Fee Operations.\n");
                        break;
                    default:
                        Console.WriteLine("\nInvalid selection. Try again.\n");
                        break;
                }
            } while (choice != "5");
        }

        /// <summary>
        /// Menu for monthly transactions
        /// </summary>
        /// <param name="admin"></param>
        public void MonthlyTransactions(DTOAdmin admin)
        {
            string choice;

            DateTime today = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            DTOMonthlyTransactions dTOMonthlyTransactions = new DTOMonthlyTransactions()
            {
                FirstDayOfMonth = firstDayOfMonth,
                ThisDayOfMonth = today,
                LastDayOfMonth = lastDayOfMonth
            };

            do
            {
                Console.WriteLine("==============================");
                Console.WriteLine("MONTHLY TRANSACTIONS");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Monthly KMV");
                Console.WriteLine("2. Monthly Transaction Volume");
                Console.WriteLine("3. Monthly Transaction Fee Total");
                Console.WriteLine("4. Back to Main Menu");
                Console.WriteLine("==============================");
                Console.Write("Make your pick (1-4): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        BAdmin bAdmin = new BAdmin();
                        bAdmin.MonthlyKMV(admin, dTOMonthlyTransactions);
                        break;
                    case "2":
                        BAdmin bAdmin1 = new BAdmin();
                        bAdmin1.MonthlyTransactionVolume(admin, dTOMonthlyTransactions);
                        break;
                    case "3":
                        BAdmin bAdmin2 = new BAdmin();
                        bAdmin2.MonthlyFeeTotal(admin, dTOMonthlyTransactions);
                        break;
                    case "4":
                        Console.WriteLine("\nExited from Fee Operations.\n");
                        break;
                    default:
                        Console.WriteLine("\nInvalid selection. Try again.\n");
                        break;
                }
            } while (choice != "5");
        }
    }
}