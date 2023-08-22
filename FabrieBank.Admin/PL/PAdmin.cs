using FabrieBank.DAL;
using FabrieBank.DAL.Entity;
using System.Reflection;
using FabrieBank.Admin.DAL.DTO;
using FabrieBank.Admin.BLL.Logic;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;
using System.Globalization;

namespace FabrieBank.Admin.PL
{
    public class PAdmin
    {
        private BAdmin bAdmin;

        public PAdmin()
        {
            bAdmin = new BAdmin();
        }

        /// <summary>
        /// Admin login module
        /// </summary>
        public void LogIn()
        {
            try
            {
                Console.WriteLine("Nickname: ");
                Console.Write(">>> ");
                string? nickname = Console.ReadLine();
                while (string.IsNullOrEmpty(nickname) || nickname.Length <= 2)
                {
                    Console.WriteLine("Nickname must be a string and longer than 2 characters.");
                    Console.Write("Nickname: ");
                    nickname = Console.ReadLine();
                }

                Console.WriteLine("Password: ");
                Console.Write(">>> ");
                int password;
                while (!int.TryParse(GetMaskedInput(), out password) || password.ToString().Length != 4)
                {
                    Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                }

                string hashedPassword = DataAccessLayer.ComputeHash(password.ToString());

                static string GetMaskedInput()
                {
                    string input = "";
                    ConsoleKeyInfo keyInfo;

                    do
                    {
                        keyInfo = Console.ReadKey(true);

                        if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
                        {
                            input += keyInfo.KeyChar;
                            Console.Write("*");
                        }
                        else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                        {
                            input = input.Substring(0, input.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                    while (keyInfo.Key != ConsoleKey.Enter);

                    return input;
                }

                DTOAdmin admin = bAdmin.LogIn(nickname, hashedPassword);

                if (admin != null)
                {
                    Console.Clear();
                    Console.WriteLine("******************************************************");
                    Console.WriteLine("*            |                         |             *");
                    Console.WriteLine("*            |    Login Successfull    |             *");
                    Console.WriteLine("*            V                         V             *");
                    Console.WriteLine("******************************************************\n");

                    Program program = new Program();
                    program.MainMenu(admin);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("**********************************************");
                    Console.WriteLine("*            !!! Login Failed !!!            *");
                    Console.WriteLine("*                                            *");
                    Console.WriteLine("*          !Wrong TCKN or Password!          *");
                    Console.WriteLine("**********************************************\n");
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Updates customer status
        /// </summary>
        public void CustomerStatusUpdate()
        {
            try
            {
                DTOCustomer customers = new DTOCustomer()
                {
                    Status = (int)EnumCustomerStatus.Pending
                };

                ECustomer eCustomer = new ECustomer();
                List<DTOCustomer> dTOCustomers = eCustomer.ReadListCustomer(customers);

                if (dTOCustomers.Count != 0)
                {
                    Console.Clear();
                    Console.WriteLine("\nPending Customers:\n");
                    for (int i = 0; i < dTOCustomers.Count; i++)
                    {
                        Console.WriteLine($"[{i}] Customer ID: {dTOCustomers[i].CustomerId}");
                        Console.WriteLine($"    Name       : {dTOCustomers[i].Name}");
                        Console.WriteLine($"    Lastname   : {dTOCustomers[i].Lastname}");
                        Console.WriteLine($"    TCKN       : {dTOCustomers[i].Tckn}");
                        Console.WriteLine($"    Cell No    : {dTOCustomers[i].CellNo}");
                        Console.WriteLine($"    Email      : {dTOCustomers[i].Email}");
                        Console.WriteLine($"    Status     : {dTOCustomers[i].Status}");
                        Console.WriteLine("==========================================\n");
                    }
                    Console.WriteLine("\nWhich customer would you like to update status?");
                    Console.Write("Customer Index: ");
                    int selectedCustomerIndex = int.Parse(Console.ReadLine());

                    if (selectedCustomerIndex >= 0 && selectedCustomerIndex < dTOCustomers.Count)
                    {
                        Console.WriteLine($"\nCurrent status of selected customer is 'Pending'.");
                        Console.WriteLine("\nDo you want to accept or decline? (Accept = 1, Decline = 2)");
                        Console.Write(">>> ");
                        string? ch = Console.ReadLine();

                        switch (ch)
                        {
                            case "1":

                                DTOCustomer acceptedCustomer = new DTOCustomer()
                                {
                                    CustomerId = dTOCustomers[selectedCustomerIndex].CustomerId,
                                    Name = dTOCustomers[selectedCustomerIndex].Name,
                                    Lastname = dTOCustomers[selectedCustomerIndex].Lastname,
                                    Password = dTOCustomers[selectedCustomerIndex].Password,
                                    CellNo = dTOCustomers[selectedCustomerIndex].CellNo,
                                    Email = dTOCustomers[selectedCustomerIndex].Email,
                                    Status = (int)EnumCustomerStatus.Active
                                };

                                bool updated = bAdmin.CustomerStatusUpdate(acceptedCustomer);

                                if (updated)
                                {
                                    Console.WriteLine("\nCustomer accepted.\n");
                                }
                                else
                                {
                                    Console.WriteLine("\nAn error occurred while updating Customer Status.\n");
                                }

                                break;

                            case "2":

                                DTOCustomer declinedCustomer = new DTOCustomer()
                                {
                                    CustomerId = dTOCustomers[selectedCustomerIndex].CustomerId,
                                    Name = dTOCustomers[selectedCustomerIndex].Name,
                                    Lastname = dTOCustomers[selectedCustomerIndex].Lastname,
                                    Password = dTOCustomers[selectedCustomerIndex].Password,
                                    CellNo = dTOCustomers[selectedCustomerIndex].CellNo,
                                    Email = dTOCustomers[selectedCustomerIndex].Email,
                                    Status = (int)EnumCustomerStatus.Inactive
                                };

                                bool updated1 = bAdmin.CustomerStatusUpdate(declinedCustomer);

                                if (updated1)
                                {
                                    Console.WriteLine("\nCustomer declined.\n");
                                }
                                else
                                {
                                    Console.WriteLine("\nAn error occurred while updating Customer Status.\n");
                                }

                                break;

                            default:

                                Console.WriteLine("Invalid Choice!");
                                break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("There are no pending customers.");
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// List customers by conditions
        /// </summary>
        /// <param name="admin"></param>
        public void ListCustomers(DTOAdmin admin)
        {
            try
            {
                if (admin.AccessLevel != 1)
                {
                    Console.WriteLine("\nTo Search By TCKN --> 1, To Search By Other Conditions --> 2");
                    Console.Write(">>> ");
                    string? ch = Console.ReadLine();

                    switch (ch)
                    {
                        case "1":

                            Console.WriteLine("\n==============================================");
                            Console.Write("Customer TCKN: ");
                            long customerTckn;
                            while (!long.TryParse(Console.ReadLine(), out customerTckn) || customerTckn.ToString().Length != 11)
                            {
                                Console.WriteLine("Invalid TCKN. Please enter a 11-digit TCKN:");
                            }

                            DTOCustomer dTOCustomer = new DTOCustomer()
                            {
                                Tckn = customerTckn
                            };

                            bAdmin.ListCustomers(dTOCustomer);

                            break;

                        case "2":

                            Console.WriteLine("\nTo list all customers please leave empty all conditions.");
                            Console.WriteLine("\n==============================================");
                            Console.Write("Customer Name: ");
                            string? customerName = Console.ReadLine();

                            Console.WriteLine("\n==============================================");
                            Console.Write("Customer Lastname: ");
                            string? customerLastname = Console.ReadLine();

                            Console.WriteLine("\n==============================================");
                            Console.Write("Customer Cell Number: ");
                            long customerCellNo;
                            while (!long.TryParse(Console.ReadLine(), out customerCellNo))
                            {
                                Console.WriteLine("Invalid Cell No. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Customer Cell No: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Customer Email: ");
                            string customerEmail = Console.ReadLine();

                            Console.WriteLine("\n==============================================");
                            Console.Write("Customer Status: ");
                            int customerStatus;
                            while (!int.TryParse(Console.ReadLine(), out customerStatus))
                            {
                                Console.WriteLine("Invalid Status. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Customer Status: \n");
                            }

                            DTOCustomer customer = new DTOCustomer
                            {
                                Name = customerName,
                                Lastname = customerLastname,
                                CellNo = customerCellNo,
                                Email = customerEmail,
                                Status = customerStatus
                            };

                            bAdmin.ListCustomers(customer);

                            break;

                        default:

                            Console.WriteLine("Invalid Choice!");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\nPermission Denied! Your Access Level Is Insufficient To Perform This Operation\n");
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// List accounts by conditions
        /// </summary>
        /// <param name="admin"></param>
        public void ListAccounts(DTOAdmin admin)
        {
            try
            {
                if (admin.AccessLevel != 1)
                {
                    Console.WriteLine("\nTo Search By Account No --> 1, To Search By Other Conditions --> 2");
                    Console.Write(">>> ");
                    string? ch = Console.ReadLine();

                    switch (ch)
                    {
                        case "1":

                            Console.WriteLine("\n==============================================");
                            Console.Write("Account No: ");
                            long accountNo;
                            while (!long.TryParse(Console.ReadLine(), out accountNo) || accountNo.ToString().Length != 10)
                            {
                                Console.WriteLine("Invalid Account No. Please enter a 11-digit Account No:");
                            }

                            DTOAccountInfo accountInfo = new DTOAccountInfo()
                            {
                                AccountNo = accountNo
                            };

                            bAdmin.ListAccounts(accountInfo);

                            break;

                        case "2":

                            Console.WriteLine("\n==============================================");
                            Console.Write("Balance Equals: ");
                            decimal balanceEquals;
                            while (!decimal.TryParse(Console.ReadLine(), out balanceEquals))
                            {
                                Console.WriteLine("Invalid Balance. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Balance Equals: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Balance Is Small: ");
                            decimal balanceIsSmall;
                            while (!decimal.TryParse(Console.ReadLine(), out balanceIsSmall))
                            {
                                Console.WriteLine("Invalid Balance. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Balance Is Small: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Balance Is Big: ");
                            decimal balanceIsBig;
                            while (!decimal.TryParse(Console.ReadLine(), out balanceIsBig))
                            {
                                Console.WriteLine("Invalid Balance. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Balance Is Big: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Customer ID: ");
                            int customerId;
                            while (!int.TryParse(Console.ReadLine(), out customerId))
                            {
                                Console.WriteLine("Invalid ID. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Customer ID: \n");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Currency Type: ");
                            int currencyType;
                            while (!int.TryParse(Console.ReadLine(), out currencyType))
                            {
                                Console.WriteLine("Invalid Currency Type. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Currency Type: \n");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Account Name: ");
                            string accountName = Console.ReadLine();

                            DTOAccountInfo dTOAccountInfo = new DTOAccountInfo
                            {
                                Balance = balanceEquals,
                                BalanceIsSmall = balanceIsSmall,
                                BalanceIsBig = balanceIsBig,
                                CustomerId = customerId,
                                CurrencyType = currencyType,
                                AccountName = accountName
                            };

                            bAdmin.ListAccounts(dTOAccountInfo);

                            break;

                        default:

                            Console.WriteLine("Invalid Choice!");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\nPermission Denied! Your Access Level Is Insufficient To Perform This Operation\n");
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// List error logs by conditions
        /// </summary>
        /// <param name="admin"></param>
        public void ListErrorLogs(DTOAdmin admin)
        {
            try
            {
                if (admin.AccessLevel != 1)
                {
                    Console.WriteLine("\nTo Search By Error ID --> 1, To Search By Other Conditions --> 2");
                    Console.Write(">>> ");
                    string? ch = Console.ReadLine();

                    switch (ch)
                    {
                        case "1":

                            Console.WriteLine("\n==============================================");
                            Console.Write("Error ID: ");
                            int errorId;
                            while (!int.TryParse(Console.ReadLine(), out errorId) || errorId <= 0)
                            {
                                Console.WriteLine("Invalid Error ID. Please enter a number greater than 0:");
                            }

                            DTOErrorLog errorLog = new DTOErrorLog()
                            {
                                ErrorId = errorId
                            };

                            bAdmin.ListErrorLogs(errorLog);

                            break;

                        case "2":

                            Console.WriteLine("\n==============================================");
                            DateTime startDate;
                            while (true)
                            {
                                Console.Write("Start Date (yyyyMMdd): ");
                                string input = Console.ReadLine();
                                if (input == "0")
                                {
                                    startDate = DateTime.MinValue;
                                    break;
                                }

                                if (DateTime.TryParseExact(input, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                                {
                                    break;
                                }

                                Console.WriteLine("Invalid Start Date format. Please enter a valid date (yyyyMMdd) or 0 to leave it empty: ");
                            }

                            Console.WriteLine("\n==============================================");
                            DateTime endDate;
                            while (true)
                            {
                                Console.Write("End Date (yyyyMMdd): ");
                                string input = Console.ReadLine();
                                if (input == "0")
                                {
                                    endDate = DateTime.MaxValue;
                                    break;
                                }

                                if (DateTime.TryParseExact(input, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                                {
                                    if (endDate >= startDate || startDate == DateTime.MinValue)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid End Date. End Date must be greater than or equal to Start Date.");
                                    }
                                }

                                Console.WriteLine("Invalid End Date format. Please enter a valid date (yyyyMMdd) or 0 to leave it empty: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Error Message: ");
                            string errorMessage = Console.ReadLine();

                            Console.WriteLine("\n==============================================");
                            Console.Write("Stack Trace: ");
                            string stackTrace = Console.ReadLine();

                            Console.WriteLine("\n==============================================");
                            Console.Write("Operation Name: ");
                            string operationName = Console.ReadLine();

                            DTOErrorLog dTOErrorLog = new DTOErrorLog
                            {
                                ErrorDateTime = startDate,
                                StartDate = startDate,
                                EndDate = endDate,
                                ErrorMessage = errorMessage,
                                StackTrace = stackTrace,
                                OperationName = operationName
                            };

                            bAdmin.ListErrorLogs(dTOErrorLog);

                            break;

                        default:

                            Console.WriteLine("Invalid Choice!");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\nPermission Denied! Your Access Level Is Insufficient To Perform This Operation\n");
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// List transaction logs by conditions
        /// </summary>
        public void ListTransactionLogs(DTOAdmin admin)
        {
            try
            {
                if (admin.AccessLevel != 1)
                {
                    Console.WriteLine("\nTo Search By Transaction ID --> 1, To Search By Other Conditions --> 2");
                    Console.Write(">>> ");
                    string? ch = Console.ReadLine();

                    switch (ch)
                    {
                        case "1":

                            Console.WriteLine("\n==============================================");
                            Console.Write("Transaction ID: ");
                            int transactionId;
                            while (!int.TryParse(Console.ReadLine(), out transactionId) || transactionId <= 0)
                            {
                                Console.WriteLine("Invalid Transaction ID. Please enter a number greater than 0:");
                            }

                            DTOTransactionLog transactionLog = new DTOTransactionLog()
                            {
                                LogId = transactionId
                            };

                            bAdmin.ListTransactionLogs(transactionLog);

                            break;

                        case "2":

                            Console.WriteLine("\n==============================================");
                            Console.Write("Source Account Number: ");
                            long sourceAccountNumber;
                            while (!long.TryParse(Console.ReadLine(), out sourceAccountNumber))
                            {
                                Console.WriteLine("Invalid Source Account Number. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Source Account Number: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Target Account Number: ");
                            long targetAccountNumber;
                            while (!long.TryParse(Console.ReadLine(), out targetAccountNumber))
                            {
                                Console.WriteLine("Invalid Target Account Number. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Target Account Number: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Transaction Type: ");
                            int transactionType;
                            while (!int.TryParse(Console.ReadLine(), out transactionType))
                            {
                                Console.WriteLine("Invalid Transaction Type. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Transaction Type: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Transaction Status: ");
                            int transactionStatus;
                            while (!int.TryParse(Console.ReadLine(), out transactionStatus))
                            {
                                Console.WriteLine("Invalid Transaction Status. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Transaction Status: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Transfer Amount Is Small: ");
                            decimal transferAmountIsSmall;
                            while (!decimal.TryParse(Console.ReadLine(), out transferAmountIsSmall))
                            {
                                Console.WriteLine("Invalid Transfer Amount . Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Transfer Amount Is Small: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Transfer Amount Is Big: ");
                            decimal transferAmountIsBig;
                            while (!decimal.TryParse(Console.ReadLine(), out transferAmountIsBig))
                            {
                                Console.WriteLine("Invalid Transfer Amount. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Transfer Amount Is Big ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("Currency Rate: ");
                            decimal currencyRate;
                            while (!decimal.TryParse(Console.ReadLine(), out currencyRate))
                            {
                                Console.WriteLine("Invalid Currency Rate. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("Currency Rate: ");
                            }

                            Console.WriteLine("\n==============================================");
                            DateTime startDate;
                            while (true)
                            {
                                Console.Write("Start Date (yyyyMMdd): ");
                                string input = Console.ReadLine();
                                if (input == "0")
                                {
                                    startDate = DateTime.MinValue;
                                    break;
                                }

                                if (DateTime.TryParseExact(input, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                                {
                                    break;
                                }

                                Console.WriteLine("Invalid Start Date format. Please enter a valid date (yyyyMMdd) or 0 to leave it empty: ");
                            }

                            Console.WriteLine("\n==============================================");
                            DateTime endDate;
                            while (true)
                            {
                                Console.Write("End Date (yyyyMMdd): ");
                                string input = Console.ReadLine();
                                if (input == "0")
                                {
                                    endDate = DateTime.MaxValue;
                                    break;
                                }

                                if (DateTime.TryParseExact(input, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                                {
                                    if (endDate >= startDate || startDate == DateTime.MinValue)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid End Date. End Date must be greater than or equal to Start Date.");
                                    }
                                }

                                Console.WriteLine("Invalid End Date format. Please enter a valid date (yyyyMMdd) or 0 to leave it empty: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("KMV Is Small: ");
                            decimal kmvIsSmall;
                            while (!decimal.TryParse(Console.ReadLine(), out kmvIsSmall))
                            {
                                Console.WriteLine("Invalid KMV. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("KMV Is Small: ");
                            }

                            Console.WriteLine("\n==============================================");
                            Console.Write("KMV Is Big: ");
                            decimal kmvIsBig;
                            while (!decimal.TryParse(Console.ReadLine(), out kmvIsBig))
                            {
                                Console.WriteLine("Invalid KMV. Please enter a numeric value or if you want to left it empty please give input 0: ");
                                Console.Write("KMV Is Big: ");
                            }

                            DTOTransactionLog dTOTransactionLog = new DTOTransactionLog
                            {
                                SourceAccountNumber = sourceAccountNumber,
                                TargetAccountNumber = targetAccountNumber,
                                TransactionType = (EnumTransactionType)transactionType,
                                TransactionStatus = (EnumTransactionStatus)transactionStatus,
                                TransferAmountSmall = transferAmountIsSmall,
                                TransferAmountLarge = transferAmountIsBig,
                                CurrencyRate = currencyRate,
                                StartDate = startDate,
                                EndDate = endDate,
                                KMVSmall = kmvIsSmall,
                                KMVLarge = kmvIsBig
                            };

                            bAdmin.ListTransactionLogs(dTOTransactionLog);

                            break;

                        default:

                            Console.WriteLine("Invalid Choice!");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\nPermission Denied! Your Access Level Is Insufficient To Perform This Operation\n");
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// List transaction logs by id
        /// </summary>
        /// <param name="admin"></param>
        public void ListFees(DTOAdmin admin)
        {
            try
            {
                if (admin.AccessLevel != 1)
                {
                    Console.WriteLine("\n==============================================");
                    Console.Write("Transaction Fee Is Small: ");
                    decimal transactionFeeIsSmall;
                    while (!decimal.TryParse(Console.ReadLine(), out transactionFeeIsSmall))
                    {
                        Console.WriteLine("Invalid Transaction Fee . Please enter a numeric value or if you want to left it empty please give input 0: ");
                        Console.Write("Transaction Fee Is Small: ");
                    }

                    Console.WriteLine("\n==============================================");
                    Console.Write("Transaction Fee Is Big: ");
                    decimal transactionFeeIsBig;
                    while (!decimal.TryParse(Console.ReadLine(), out transactionFeeIsBig))
                    {
                        Console.WriteLine("Invalid Transaction Fee. Please enter a numeric value or if you want to left it empty please give input 0: ");
                        Console.Write("Transaction Fee Is Big: ");
                    }

                    DTOTransactionFee dTOTransactionFee = new DTOTransactionFee()
                    {
                        AmountIsSmall = transactionFeeIsSmall,
                        AmountIsBig = transactionFeeIsBig
                    };

                    bAdmin.ListFees(dTOTransactionFee);
                }
                else
                {
                    Console.WriteLine("\nPermission Denied! Your Access Level Is Insufficient To Perform This Operation\n");
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Add new fee with fee amount input
        /// </summary>
        /// <param name="admin"></param>
        public void AddFee(DTOAdmin admin)
        {
            try
            {
                if (admin.AccessLevel == 3)
                {
                    Console.WriteLine("\n==============================================");
                    Console.Write("Fee Amount or Percentage: ");
                    decimal amountInput;
                    while (!decimal.TryParse(Console.ReadLine(), out amountInput))
                    {
                        Console.WriteLine("Invalid Transaction Fee . Please enter a numeric value");
                        Console.Write("Fee Amount or Percentage: ");
                    }
                    DTOTransactionFee dTOTransactionFee = new DTOTransactionFee()
                    {
                        Amount = amountInput
                    };
                    bool inserted = bAdmin.AddFee(dTOTransactionFee);
                    if (inserted)
                    {
                        Console.WriteLine("\nNew fee has been added successfuly.\n");
                    }
                    else
                    {
                        Console.WriteLine("\nSomething went wrong while adding new fee.\n");
                    }
                }
                else
                {
                    Console.WriteLine("\nPermission Denied! Your Access Level Is Insufficient To Perform This Operation\n");
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Remove fee with fee id
        /// </summary>
        /// <param name="admin"></param>
        public void RemoveFee(DTOAdmin admin)
        {
            try
            {
                if (admin.AccessLevel == 3)
                {
                    Console.WriteLine("\n==============================================");
                    Console.Write("Fee ID: ");
                    int idInput;
                    while (!int.TryParse(Console.ReadLine(), out idInput))
                    {
                        Console.WriteLine("Invalid Fee ID. Please enter a numeric value");
                        Console.Write("Fee ID: ");
                    }
                    DTOTransactionFee dTOTransactionFee = new DTOTransactionFee()
                    {
                        FeeType = (EnumTransactionFeeType)idInput
                    };
                    bool deleted = bAdmin.RemoveFee(dTOTransactionFee);
                    if (deleted)
                    {
                        Console.WriteLine("\nFee has been deleted successfuly.\n");
                    }
                    else
                    {
                        Console.WriteLine("\nSomething went wrong while deleting fee.\n");
                    }
                }
                else
                {
                    Console.WriteLine("\nPermission Denied! Your Access Level Is Insufficient To Perform This Operation\n");
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Update fee with id and amount input
        /// </summary>
        /// <param name="admin"></param>
        public void UpdateFee(DTOAdmin admin)
        {
            try
            {
                if (admin.AccessLevel == 3)
                {
                    Console.WriteLine("\n==============================================");
                    Console.Write("Fee ID: ");
                    int idInput;
                    while (!int.TryParse(Console.ReadLine(), out idInput))
                    {
                        Console.WriteLine("Invalid Fee ID. Please enter a numeric value");
                        Console.Write("Fee ID: ");
                    }

                    Console.WriteLine("\n==============================================");
                    Console.Write("Fee Amount or Percentage: ");
                    decimal amountInput;
                    while (!decimal.TryParse(Console.ReadLine(), out amountInput))
                    {
                        Console.WriteLine("Invalid Transaction Fee . Please enter a numeric value");
                        Console.Write("Fee Amount or Percentage: ");
                    }

                    DTOTransactionFee dTOTransactionFee = new DTOTransactionFee()
                    {
                        FeeType = (EnumTransactionFeeType)idInput,
                        Amount = amountInput
                    };
                    bool updated = bAdmin.UpdateFee(dTOTransactionFee);
                    if (updated)
                    {
                        Console.WriteLine("\nFee has been updated successfuly.\n");
                    }
                    else
                    {
                        Console.WriteLine("\nSomething went wrong while updating fee.\n");
                    }
                }
                else
                {
                    Console.WriteLine("\nPermission Denied! Your Access Level Is Insufficient To Perform This Operation\n");
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }
    }
}