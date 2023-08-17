using System.Reflection;
using FabrieBank.BLL.Service;
using FabrieBank.DAL;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;
using FabrieBank.DAL.Entity;

namespace FabrieBank.BLL.Logic
{
    public class BTransaction
    {
        private DataAccessLayer dataAccessLayer;
        private EAccountInfo eAccount;
        private BAccount account;
        private ETransactionLog eTransactionLog;
        private ETransactionFee eTransactionFee;

        public BTransaction()
        {
            eAccount = new EAccountInfo();
            account = new BAccount();
            dataAccessLayer = new DataAccessLayer();
            eTransactionLog = new ETransactionLog();
            eTransactionFee = new ETransactionFee();
        }

        /// <summary>
        /// Money deposit process
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <param name="balance"></param>
        public void Deposit(DTOAccountInfo accountInfo, decimal balance)
        {
            decimal transactionFee = eTransactionFee.ReadTransactionFee(EnumTransactionFeeType.Deposit);
            accountInfo = eAccount.ReadAccountInfo(accountInfo);
            if (accountInfo.CustomerId != 0)
            {
                if (accountInfo.CurrencyType == 1)
                {
                    if (balance <= 10000M)
                    {
                        decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                        decimal newBalance = oldBalance + balance;

                        DTOAccountInfo dTOAccount = new DTOAccountInfo()
                        {
                            AccountNo = accountInfo.AccountNo,
                            Balance = newBalance,
                            AccountName = accountInfo.AccountName
                        };

                        eAccount.UpdateAccountInfo(dTOAccount);

                        // Log the successful deposit
                        DTOTransactionLog transactionLog = new DTOTransactionLog
                        {
                            TargetAccountNumber = accountInfo.AccountNo,
                            TransactionType = EnumTransactionType.Deposit,
                            TransactionStatus = EnumTransactionStatus.Success,
                            TransferAmount = newBalance - oldBalance,
                            TargetOldBalance = oldBalance,
                            TargetNewBalance = newBalance,
                            Timestamp = DateTime.Now,
                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                        };

                        eTransactionLog.InsertTransactionLog(transactionLog);

                        Console.WriteLine("\nDeposit successful.");
                        Console.WriteLine($"Old balance: {oldBalance}");
                        Console.WriteLine($"New balance: {newBalance}");
                    }
                    else
                    {
                        Console.WriteLine($"You entered a request over the daily deposit limit (10000). If you continue with the transaction, a transaction fee of {transactionFee} will be charged.");
                        Console.WriteLine("\nDo you want to continue? 1-) Yes, 2-) No");
                        Console.Write(">>> ");
                        string? ch = Console.ReadLine();

                        switch (ch)
                        {
                            case "1":

                                decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                                decimal newBalance = oldBalance + balance - transactionFee;

                                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                                {
                                    AccountNo = accountInfo.AccountNo,
                                    Balance = newBalance,
                                    AccountName = accountInfo.AccountName
                                };

                                eAccount.UpdateAccountInfo(dTOAccount);

                                // Log the successful deposit
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    TargetAccountNumber = accountInfo.AccountNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Success,
                                    TransferAmount = newBalance - oldBalance,
                                    TargetOldBalance = oldBalance,
                                    TargetNewBalance = newBalance,
                                    Timestamp = DateTime.Now,
                                    TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    TransactionFee = transactionFee
                                };

                                eTransactionLog.InsertTransactionLog(transactionLog);

                                Console.WriteLine("\nDeposit successful.");
                                Console.WriteLine($"Old balance: {oldBalance}");
                                Console.WriteLine($"New balance: {newBalance}");

                                break;

                            case "2":

                                // Log the failed deposit
                                DTOTransactionLog transactionLogTRY = new DTOTransactionLog
                                {
                                    TargetAccountNumber = accountInfo.AccountNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Failed,
                                    TransferAmount = balance,
                                    TargetOldBalance = accountInfo.Balance,
                                    TargetNewBalance = accountInfo.Balance,
                                    Timestamp = DateTime.Now,
                                    TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                };

                                eTransactionLog.InsertTransactionLog(transactionLogTRY);

                                Console.WriteLine("\nDeposit unsuccessful.");

                                break;

                            default:

                                Console.WriteLine("Invalid Choice!");
                                break;
                        }
                    }
                }
                else if (accountInfo.CurrencyType == 2 || accountInfo.CurrencyType == 4)
                {
                    if (balance <= 5000M)
                    {
                        decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                        decimal newBalance = oldBalance + balance;

                        DTOAccountInfo dTOAccount = new DTOAccountInfo()
                        {
                            AccountNo = accountInfo.AccountNo,
                            Balance = newBalance,
                            AccountName = accountInfo.AccountName
                        };

                        eAccount.UpdateAccountInfo(dTOAccount);

                        // Log the successful deposit
                        DTOTransactionLog transactionLog = new DTOTransactionLog
                        {
                            TargetAccountNumber = accountInfo.AccountNo,
                            TransactionType = EnumTransactionType.Deposit,
                            TransactionStatus = EnumTransactionStatus.Success,
                            TransferAmount = newBalance - oldBalance,
                            TargetOldBalance = oldBalance,
                            TargetNewBalance = newBalance,
                            Timestamp = DateTime.Now,
                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                        };

                        eTransactionLog.InsertTransactionLog(transactionLog);

                        Console.WriteLine("\nDeposit successful.");
                        Console.WriteLine($"Old balance: {oldBalance}");
                        Console.WriteLine($"New balance: {newBalance}");
                    }
                    else
                    {
                        Console.WriteLine($"\nYou entered a request over the daily deposit limit (5000). If you continue with the transaction, a transaction fee of {transactionFee} will be charged.");
                        Console.WriteLine("\nDo you want to continue? 1-) Yes, 2-) No");
                        Console.Write(">>> ");
                        string? ch = Console.ReadLine();

                        switch (ch)
                        {
                            case "1":

                                decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                                decimal newBalance = oldBalance + balance - transactionFee;

                                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                                {
                                    AccountNo = accountInfo.AccountNo,
                                    Balance = newBalance,
                                    AccountName = accountInfo.AccountName
                                };

                                eAccount.UpdateAccountInfo(dTOAccount);

                                // Log the successful deposit
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    TargetAccountNumber = accountInfo.AccountNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Success,
                                    TransferAmount = newBalance - oldBalance,
                                    TargetOldBalance = oldBalance,
                                    TargetNewBalance = newBalance,
                                    Timestamp = DateTime.Now,
                                    TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    TransactionFee = transactionFee
                                };

                                eTransactionLog.InsertTransactionLog(transactionLog);

                                Console.WriteLine("\nDeposit successful.");
                                Console.WriteLine($"Old balance: {oldBalance}");
                                Console.WriteLine($"New balance: {newBalance}");

                                break;

                            case "2":

                                // Log the failed deposit
                                DTOTransactionLog transactionLogTRY = new DTOTransactionLog
                                {
                                    TargetAccountNumber = accountInfo.AccountNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Failed,
                                    TransferAmount = balance,
                                    TargetOldBalance = accountInfo.Balance,
                                    TargetNewBalance = accountInfo.Balance,
                                    Timestamp = DateTime.Now,
                                    TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                };

                                eTransactionLog.InsertTransactionLog(transactionLogTRY);

                                Console.WriteLine("\nDeposit unsuccessful.");

                                break;

                            default:

                                Console.WriteLine("Invalid Choice!");
                                break;
                        }
                    }
                }
                else if (accountInfo.CurrencyType == 3)
                {
                    if (balance <= 25000M)
                    {
                        decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                        decimal newBalance = oldBalance + balance;

                        DTOAccountInfo dTOAccount = new DTOAccountInfo()
                        {
                            AccountNo = accountInfo.AccountNo,
                            Balance = newBalance,
                            AccountName = accountInfo.AccountName
                        };

                        eAccount.UpdateAccountInfo(dTOAccount);

                        // Log the successful deposit
                        DTOTransactionLog transactionLog = new DTOTransactionLog
                        {
                            TargetAccountNumber = accountInfo.AccountNo,
                            TransactionType = EnumTransactionType.Deposit,
                            TransactionStatus = EnumTransactionStatus.Success,
                            TransferAmount = newBalance - oldBalance,
                            TargetOldBalance = oldBalance,
                            TargetNewBalance = newBalance,
                            Timestamp = DateTime.Now,
                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                        };

                        eTransactionLog.InsertTransactionLog(transactionLog);

                        Console.WriteLine("\nDeposit successful.");
                        Console.WriteLine($"Old balance: {oldBalance}");
                        Console.WriteLine($"New balance: {newBalance}");
                    }
                    else
                    {
                        Console.WriteLine($"\nYou entered a request over the daily deposit limit (25000). If you continue with the transaction, a transaction fee of {transactionFee} will be charged.");
                        Console.WriteLine("\nDo you want to continue? 1-) Yes, 2-) No");
                        Console.Write(">>> ");
                        string? ch = Console.ReadLine();

                        switch (ch)
                        {
                            case "1":

                                decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                                decimal newBalance = oldBalance + balance - transactionFee;

                                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                                {
                                    AccountNo = accountInfo.AccountNo,
                                    Balance = newBalance,
                                    AccountName = accountInfo.AccountName
                                };

                                eAccount.UpdateAccountInfo(dTOAccount);

                                // Log the successful deposit
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    TargetAccountNumber = accountInfo.AccountNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Success,
                                    TransferAmount = newBalance - oldBalance,
                                    TargetOldBalance = oldBalance,
                                    TargetNewBalance = newBalance,
                                    Timestamp = DateTime.Now,
                                    TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    TransactionFee = transactionFee
                                };

                                eTransactionLog.InsertTransactionLog(transactionLog);

                                Console.WriteLine("\nDeposit successful.");
                                Console.WriteLine($"Old balance: {oldBalance}");
                                Console.WriteLine($"New balance: {newBalance}");

                                break;

                            case "2":

                                // Log the failed deposit
                                DTOTransactionLog transactionLogTRY = new DTOTransactionLog
                                {
                                    TargetAccountNumber = accountInfo.AccountNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Failed,
                                    TransferAmount = balance,
                                    TargetOldBalance = accountInfo.Balance,
                                    TargetNewBalance = accountInfo.Balance,
                                    Timestamp = DateTime.Now,
                                    TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                };

                                eTransactionLog.InsertTransactionLog(transactionLogTRY);

                                Console.WriteLine("\nDeposit unsuccessful.");

                                break;

                            default:

                                Console.WriteLine("Invalid Choice!");
                                break;
                        }
                    }
                }
                else if (accountInfo.CurrencyType == 5)
                {
                    if (balance <= 50000M)
                    {
                        decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                        decimal newBalance = oldBalance + balance;

                        DTOAccountInfo dTOAccount = new DTOAccountInfo()
                        {
                            AccountNo = accountInfo.AccountNo,
                            Balance = newBalance,
                            AccountName = accountInfo.AccountName
                        };

                        eAccount.UpdateAccountInfo(dTOAccount);

                        // Log the successful deposit
                        DTOTransactionLog transactionLog = new DTOTransactionLog
                        {
                            TargetAccountNumber = accountInfo.AccountNo,
                            TransactionType = EnumTransactionType.Deposit,
                            TransactionStatus = EnumTransactionStatus.Success,
                            TransferAmount = newBalance - oldBalance,
                            TargetOldBalance = oldBalance,
                            TargetNewBalance = newBalance,
                            Timestamp = DateTime.Now,
                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                        };

                        eTransactionLog.InsertTransactionLog(transactionLog);

                        Console.WriteLine("\nDeposit successful.");
                        Console.WriteLine($"Old balance: {oldBalance}");
                        Console.WriteLine($"New balance: {newBalance}");
                    }
                    else
                    {
                        Console.WriteLine($"\nYou entered a request over the daily deposit limit (50000). If you continue with the transaction, a transaction fee of {transactionFee} will be charged.");
                        Console.WriteLine("\nDo you want to continue? 1-) Yes, 2-) No");
                        Console.Write(">>> ");
                        string? ch = Console.ReadLine();

                        switch (ch)
                        {
                            case "1":

                                decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                                decimal newBalance = oldBalance + balance - transactionFee;

                                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                                {
                                    AccountNo = accountInfo.AccountNo,
                                    Balance = newBalance,
                                    AccountName = accountInfo.AccountName
                                };

                                eAccount.UpdateAccountInfo(dTOAccount);

                                // Log the successful deposit
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    TargetAccountNumber = accountInfo.AccountNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Success,
                                    TransferAmount = newBalance - oldBalance,
                                    TargetOldBalance = oldBalance,
                                    TargetNewBalance = newBalance,
                                    Timestamp = DateTime.Now,
                                    TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    TransactionFee = transactionFee
                                };

                                eTransactionLog.InsertTransactionLog(transactionLog);

                                Console.WriteLine("\nDeposit successful.");
                                Console.WriteLine($"Old balance: {oldBalance}");
                                Console.WriteLine($"New balance: {newBalance}");

                                break;

                            case "2":

                                // Log the failed deposit
                                DTOTransactionLog transactionLogTRY = new DTOTransactionLog
                                {
                                    TargetAccountNumber = accountInfo.AccountNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Failed,
                                    TransferAmount = balance,
                                    TargetOldBalance = accountInfo.Balance,
                                    TargetNewBalance = accountInfo.Balance,
                                    Timestamp = DateTime.Now,
                                    TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                    SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                };

                                eTransactionLog.InsertTransactionLog(transactionLogTRY);

                                Console.WriteLine("\nDeposit unsuccessful.");

                                break;

                            default:

                                Console.WriteLine("Invalid Choice!");
                                break;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"\nThe account number you entered does not belong to our bank. If you continue the transaction, a transaction fee of {transactionFee} will be charged.");
                Console.WriteLine("\nDo you want to continue? 1-) Yes, 2-) No");
                Console.Write(">>> ");
                string? ch = Console.ReadLine();

                switch (ch)
                {
                    case "1":
                        // Log the successful deposit
                        DTOTransactionLog transactionLogs = new DTOTransactionLog
                        {
                            TargetAccountNumber = accountInfo.AccountNo,
                            TransactionType = EnumTransactionType.DepositToAnother,
                            TransactionStatus = EnumTransactionStatus.Success,
                            TransferAmount = balance,
                            Timestamp = DateTime.Now,
                            TransactionFee = transactionFee
                        };

                        eTransactionLog.InsertTransactionLog(transactionLogs);

                        Console.WriteLine("\nDeposit successful.");

                        break;

                    case "2":

                        // Log the failed deposit
                        DTOTransactionLog transactionLogTRY = new DTOTransactionLog
                        {
                            TargetAccountNumber = accountInfo.AccountNo,
                            TransactionType = EnumTransactionType.DepositToAnother,
                            TransactionStatus = EnumTransactionStatus.Failed,
                            TransferAmount = balance,
                            Timestamp = DateTime.Now,
                        };

                        eTransactionLog.InsertTransactionLog(transactionLogTRY);

                        Console.WriteLine("\nDeposit unsuccessful.");

                        break;

                    default:

                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }

        /// <summary>
        /// Money withdraw process
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <param name="balance"></param>
        public void Withdraw(DTOAccountInfo accountInfo, decimal balance)
        {
            decimal transactionFee = eTransactionFee.ReadTransactionFee(EnumTransactionFeeType.Withdraw);
            accountInfo = eAccount.ReadAccountInfo(accountInfo);
            if (accountInfo.CustomerId != 0)
            {
                if (accountInfo.Balance >= balance)
                {
                    if (accountInfo.CurrencyType == 1)
                    {
                        if (balance <= 5000M)
                        {
                            decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                            decimal newBalance = oldBalance - balance;

                            DTOAccountInfo dTOAccount = new DTOAccountInfo()
                            {
                                AccountNo = accountInfo.AccountNo,
                                Balance = newBalance,
                                AccountName = accountInfo.AccountName
                            };

                            eAccount.UpdateAccountInfo(dTOAccount);

                            // Log the successful deposit
                            DTOTransactionLog transactionLog = new DTOTransactionLog
                            {
                                TargetAccountNumber = accountInfo.AccountNo,
                                TransactionType = EnumTransactionType.Withdrawal,
                                TransactionStatus = EnumTransactionStatus.Success,
                                TransferAmount = oldBalance - newBalance,
                                TargetOldBalance = oldBalance,
                                TargetNewBalance = newBalance,
                                Timestamp = DateTime.Now,
                                TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                            };

                            eTransactionLog.InsertTransactionLog(transactionLog);

                            Console.WriteLine("\nWithdraw successful.");
                            Console.WriteLine($"Old balance: {oldBalance}");
                            Console.WriteLine($"New balance: {newBalance}");
                        }
                        else
                        {
                            Console.WriteLine($"You entered a request over the daily withdraw limit (5000). If you continue with the transaction, a transaction fee of {transactionFee} will be charged.");
                            Console.WriteLine("\nDo you want to continue? 1-) Yes, 2-) No");
                            Console.Write(">>> ");
                            string? ch = Console.ReadLine();

                            switch (ch)
                            {
                                case "1":

                                    if (accountInfo.Balance >= balance + transactionFee)
                                    {
                                        decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                                        decimal newBalance = oldBalance - balance - transactionFee;

                                        DTOAccountInfo dTOAccount = new DTOAccountInfo()
                                        {
                                            AccountNo = accountInfo.AccountNo,
                                            Balance = newBalance,
                                            AccountName = accountInfo.AccountName
                                        };

                                        eAccount.UpdateAccountInfo(dTOAccount);

                                        // Log the successful deposit
                                        DTOTransactionLog transactionLog = new DTOTransactionLog
                                        {
                                            TargetAccountNumber = accountInfo.AccountNo,
                                            TransactionType = EnumTransactionType.Withdrawal,
                                            TransactionStatus = EnumTransactionStatus.Success,
                                            TransferAmount = oldBalance - newBalance,
                                            TargetOldBalance = oldBalance,
                                            TargetNewBalance = newBalance,
                                            Timestamp = DateTime.Now,
                                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            TransactionFee = transactionFee
                                        };

                                        eTransactionLog.InsertTransactionLog(transactionLog);

                                        Console.WriteLine("\nWithdraw successful.");
                                        Console.WriteLine($"Old balance: {oldBalance}");
                                        Console.WriteLine($"New balance: {newBalance}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nInsufficient balance!");
                                        // Log the failed deposit
                                        DTOTransactionLog transactionLogTRYfailed = new DTOTransactionLog
                                        {
                                            TargetAccountNumber = accountInfo.AccountNo,
                                            TransactionType = EnumTransactionType.Withdrawal,
                                            TransactionStatus = EnumTransactionStatus.Failed,
                                            TransferAmount = balance,
                                            TargetOldBalance = accountInfo.Balance,
                                            TargetNewBalance = accountInfo.Balance,
                                            Timestamp = DateTime.Now,
                                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                        };

                                        eTransactionLog.InsertTransactionLog(transactionLogTRYfailed);

                                        Console.WriteLine("\nWithdraw unsuccessful.");
                                    }
                                    break;

                                case "2":

                                    // Log the failed deposit
                                    DTOTransactionLog transactionLogTRY = new DTOTransactionLog
                                    {
                                        TargetAccountNumber = accountInfo.AccountNo,
                                        TransactionType = EnumTransactionType.Withdrawal,
                                        TransactionStatus = EnumTransactionStatus.Failed,
                                        TransferAmount = balance,
                                        TargetOldBalance = accountInfo.Balance,
                                        TargetNewBalance = accountInfo.Balance,
                                        Timestamp = DateTime.Now,
                                        TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                        SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                    };

                                    eTransactionLog.InsertTransactionLog(transactionLogTRY);

                                    Console.WriteLine("\nWithdraw unsuccessful.");

                                    break;

                                default:

                                    Console.WriteLine("Invalid Choice!");
                                    break;
                            }
                        }
                    }
                    else if (accountInfo.CurrencyType == 2 || accountInfo.CurrencyType == 4)
                    {
                        if (balance <= 2500M)
                        {
                            decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                            decimal newBalance = oldBalance - balance;

                            DTOAccountInfo dTOAccount = new DTOAccountInfo()
                            {
                                AccountNo = accountInfo.AccountNo,
                                Balance = newBalance,
                                AccountName = accountInfo.AccountName
                            };

                            eAccount.UpdateAccountInfo(dTOAccount);

                            // Log the successful deposit
                            DTOTransactionLog transactionLog = new DTOTransactionLog
                            {
                                TargetAccountNumber = accountInfo.AccountNo,
                                TransactionType = EnumTransactionType.Withdrawal,
                                TransactionStatus = EnumTransactionStatus.Success,
                                TransferAmount = oldBalance - newBalance,
                                TargetOldBalance = oldBalance,
                                TargetNewBalance = newBalance,
                                Timestamp = DateTime.Now,
                                TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                            };

                            eTransactionLog.InsertTransactionLog(transactionLog);

                            Console.WriteLine("\nWithdraw successful.");
                            Console.WriteLine($"Old balance: {oldBalance}");
                            Console.WriteLine($"New balance: {newBalance}");
                        }
                        else
                        {
                            Console.WriteLine($"\nYou entered a request over the daily withdraw limit (2500). If you continue with the transaction, a transaction fee of {transactionFee} will be charged.");
                            Console.WriteLine("\nDo you want to continue? 1-) Yes, 2-) No");
                            Console.Write(">>> ");
                            string? ch = Console.ReadLine();

                            switch (ch)
                            {
                                case "1":

                                    if (accountInfo.Balance >= balance + transactionFee)
                                    {
                                        decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                                        decimal newBalance = oldBalance - balance - transactionFee;

                                        DTOAccountInfo dTOAccount = new DTOAccountInfo()
                                        {
                                            AccountNo = accountInfo.AccountNo,
                                            Balance = newBalance,
                                            AccountName = accountInfo.AccountName
                                        };

                                        eAccount.UpdateAccountInfo(dTOAccount);

                                        // Log the successful deposit
                                        DTOTransactionLog transactionLog = new DTOTransactionLog
                                        {
                                            TargetAccountNumber = accountInfo.AccountNo,
                                            TransactionType = EnumTransactionType.Withdrawal,
                                            TransactionStatus = EnumTransactionStatus.Success,
                                            TransferAmount = oldBalance - newBalance,
                                            TargetOldBalance = oldBalance,
                                            TargetNewBalance = newBalance,
                                            Timestamp = DateTime.Now,
                                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            TransactionFee = transactionFee
                                        };

                                        eTransactionLog.InsertTransactionLog(transactionLog);

                                        Console.WriteLine("\nWithdraw successful.");
                                        Console.WriteLine($"Old balance: {oldBalance}");
                                        Console.WriteLine($"New balance: {newBalance}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nInsufficient balance!");
                                        // Log the failed deposit
                                        DTOTransactionLog transactionLogTRYfailed = new DTOTransactionLog
                                        {
                                            TargetAccountNumber = accountInfo.AccountNo,
                                            TransactionType = EnumTransactionType.Withdrawal,
                                            TransactionStatus = EnumTransactionStatus.Failed,
                                            TransferAmount = balance,
                                            TargetOldBalance = accountInfo.Balance,
                                            TargetNewBalance = accountInfo.Balance,
                                            Timestamp = DateTime.Now,
                                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                        };

                                        eTransactionLog.InsertTransactionLog(transactionLogTRYfailed);

                                        Console.WriteLine("\nWithdraw unsuccessful.");
                                    }
                                    break;

                                case "2":

                                    // Log the failed deposit
                                    DTOTransactionLog transactionLogTRY = new DTOTransactionLog
                                    {
                                        TargetAccountNumber = accountInfo.AccountNo,
                                        TransactionType = EnumTransactionType.Withdrawal,
                                        TransactionStatus = EnumTransactionStatus.Failed,
                                        TransferAmount = balance,
                                        TargetOldBalance = accountInfo.Balance,
                                        TargetNewBalance = accountInfo.Balance,
                                        Timestamp = DateTime.Now,
                                        TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                        SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                    };

                                    eTransactionLog.InsertTransactionLog(transactionLogTRY);

                                    Console.WriteLine("\nWithdraw unsuccessful.");

                                    break;

                                default:

                                    Console.WriteLine("Invalid Choice!");
                                    break;
                            }
                        }
                    }
                    else if (accountInfo.CurrencyType == 3)
                    {
                        if (balance <= 12500M)
                        {
                            decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                            decimal newBalance = oldBalance - balance;

                            DTOAccountInfo dTOAccount = new DTOAccountInfo()
                            {
                                AccountNo = accountInfo.AccountNo,
                                Balance = newBalance,
                                AccountName = accountInfo.AccountName
                            };

                            eAccount.UpdateAccountInfo(dTOAccount);

                            // Log the successful deposit
                            DTOTransactionLog transactionLog = new DTOTransactionLog
                            {
                                TargetAccountNumber = accountInfo.AccountNo,
                                TransactionType = EnumTransactionType.Withdrawal,
                                TransactionStatus = EnumTransactionStatus.Success,
                                TransferAmount = oldBalance - newBalance,
                                TargetOldBalance = oldBalance,
                                TargetNewBalance = newBalance,
                                Timestamp = DateTime.Now,
                                TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                            };

                            eTransactionLog.InsertTransactionLog(transactionLog);

                            Console.WriteLine("\nWithdraw successful.");
                            Console.WriteLine($"Old balance: {oldBalance}");
                            Console.WriteLine($"New balance: {newBalance}");
                        }
                        else
                        {
                            Console.WriteLine($"\nYou entered a request over the daily withdraw limit (12500). If you continue with the transaction, a transaction fee of {transactionFee} will be charged.");
                            Console.WriteLine("\nDo you want to continue? 1-) Yes, 2-) No");
                            Console.Write(">>> ");
                            string? ch = Console.ReadLine();

                            switch (ch)
                            {
                                case "1":

                                    if (accountInfo.Balance >= balance + transactionFee)
                                    {
                                        decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                                        decimal newBalance = oldBalance - balance - transactionFee;

                                        DTOAccountInfo dTOAccount = new DTOAccountInfo()
                                        {
                                            AccountNo = accountInfo.AccountNo,
                                            Balance = newBalance,
                                            AccountName = accountInfo.AccountName
                                        };

                                        eAccount.UpdateAccountInfo(dTOAccount);

                                        // Log the successful deposit
                                        DTOTransactionLog transactionLog = new DTOTransactionLog
                                        {
                                            TargetAccountNumber = accountInfo.AccountNo,
                                            TransactionType = EnumTransactionType.Withdrawal,
                                            TransactionStatus = EnumTransactionStatus.Success,
                                            TransferAmount = oldBalance - newBalance,
                                            TargetOldBalance = oldBalance,
                                            TargetNewBalance = newBalance,
                                            Timestamp = DateTime.Now,
                                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            TransactionFee = transactionFee
                                        };

                                        eTransactionLog.InsertTransactionLog(transactionLog);

                                        Console.WriteLine("\nWithdraw successful.");
                                        Console.WriteLine($"Old balance: {oldBalance}");
                                        Console.WriteLine($"New balance: {newBalance}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nInsufficient balance!");
                                        // Log the failed deposit
                                        DTOTransactionLog transactionLogTRYfailed = new DTOTransactionLog
                                        {
                                            TargetAccountNumber = accountInfo.AccountNo,
                                            TransactionType = EnumTransactionType.Withdrawal,
                                            TransactionStatus = EnumTransactionStatus.Failed,
                                            TransferAmount = balance,
                                            TargetOldBalance = accountInfo.Balance,
                                            TargetNewBalance = accountInfo.Balance,
                                            Timestamp = DateTime.Now,
                                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                        };

                                        eTransactionLog.InsertTransactionLog(transactionLogTRYfailed);

                                        Console.WriteLine("\nWithdraw unsuccessful.");
                                    }
                                    break;

                                case "2":

                                    // Log the failed deposit
                                    DTOTransactionLog transactionLogTRY = new DTOTransactionLog
                                    {
                                        TargetAccountNumber = accountInfo.AccountNo,
                                        TransactionType = EnumTransactionType.Withdrawal,
                                        TransactionStatus = EnumTransactionStatus.Failed,
                                        TransferAmount = balance,
                                        TargetOldBalance = accountInfo.Balance,
                                        TargetNewBalance = accountInfo.Balance,
                                        Timestamp = DateTime.Now,
                                        TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                        SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                    };

                                    eTransactionLog.InsertTransactionLog(transactionLogTRY);

                                    Console.WriteLine("\nWithdraw unsuccessful.");

                                    break;

                                default:

                                    Console.WriteLine("Invalid Choice!");
                                    break;
                            }
                        }
                    }
                    else if (accountInfo.CurrencyType == 5)
                    {
                        if (balance <= 25000M)
                        {
                            decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                            decimal newBalance = oldBalance - balance;

                            DTOAccountInfo dTOAccount = new DTOAccountInfo()
                            {
                                AccountNo = accountInfo.AccountNo,
                                Balance = newBalance,
                                AccountName = accountInfo.AccountName
                            };

                            eAccount.UpdateAccountInfo(dTOAccount);

                            // Log the successful deposit
                            DTOTransactionLog transactionLog = new DTOTransactionLog
                            {
                                TargetAccountNumber = accountInfo.AccountNo,
                                TransactionType = EnumTransactionType.Withdrawal,
                                TransactionStatus = EnumTransactionStatus.Success,
                                TransferAmount = oldBalance - newBalance,
                                TargetOldBalance = oldBalance,
                                TargetNewBalance = newBalance,
                                Timestamp = DateTime.Now,
                                TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                            };

                            eTransactionLog.InsertTransactionLog(transactionLog);

                            Console.WriteLine("\nWithdraw successful.");
                            Console.WriteLine($"Old balance: {oldBalance}");
                            Console.WriteLine($"New balance: {newBalance}");
                        }
                        else
                        {
                            Console.WriteLine($"\nYou entered a request over the daily withdraw limit (25000). If you continue with the transaction, a transaction fee of {transactionFee} will be charged.");
                            Console.WriteLine("\nDo you want to continue? 1-) Yes, 2-) No");
                            Console.Write(">>> ");
                            string? ch = Console.ReadLine();

                            switch (ch)
                            {
                                case "1":

                                    if (accountInfo.Balance >= balance + transactionFee)
                                    {
                                        decimal oldBalance = Convert.ToDecimal(accountInfo.Balance);
                                        decimal newBalance = oldBalance - balance - transactionFee;

                                        DTOAccountInfo dTOAccount = new DTOAccountInfo()
                                        {
                                            AccountNo = accountInfo.AccountNo,
                                            Balance = newBalance,
                                            AccountName = accountInfo.AccountName
                                        };

                                        eAccount.UpdateAccountInfo(dTOAccount);

                                        // Log the successful deposit
                                        DTOTransactionLog transactionLog = new DTOTransactionLog
                                        {
                                            TargetAccountNumber = accountInfo.AccountNo,
                                            TransactionType = EnumTransactionType.Withdrawal,
                                            TransactionStatus = EnumTransactionStatus.Success,
                                            TransferAmount = oldBalance - newBalance,
                                            TargetOldBalance = oldBalance,
                                            TargetNewBalance = newBalance,
                                            Timestamp = DateTime.Now,
                                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            TransactionFee = transactionFee
                                        };

                                        eTransactionLog.InsertTransactionLog(transactionLog);

                                        Console.WriteLine("\nWithdraw successful.");
                                        Console.WriteLine($"Old balance: {oldBalance}");
                                        Console.WriteLine($"New balance: {newBalance}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nInsufficient balance!");
                                        // Log the failed deposit
                                        DTOTransactionLog transactionLogTRYfailed = new DTOTransactionLog
                                        {
                                            TargetAccountNumber = accountInfo.AccountNo,
                                            TransactionType = EnumTransactionType.Withdrawal,
                                            TransactionStatus = EnumTransactionStatus.Failed,
                                            TransferAmount = balance,
                                            TargetOldBalance = accountInfo.Balance,
                                            TargetNewBalance = accountInfo.Balance,
                                            Timestamp = DateTime.Now,
                                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                        };

                                        eTransactionLog.InsertTransactionLog(transactionLogTRYfailed);

                                        Console.WriteLine("\nWithdraw unsuccessful.");
                                    }
                                    break;

                                case "2":

                                    // Log the failed deposit
                                    DTOTransactionLog transactionLogTRY = new DTOTransactionLog
                                    {
                                        TargetAccountNumber = accountInfo.AccountNo,
                                        TransactionType = EnumTransactionType.Withdrawal,
                                        TransactionStatus = EnumTransactionStatus.Failed,
                                        TransferAmount = balance,
                                        TargetOldBalance = accountInfo.Balance,
                                        TargetNewBalance = accountInfo.Balance,
                                        Timestamp = DateTime.Now,
                                        TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType,
                                        SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)accountInfo.CurrencyType
                                    };

                                    eTransactionLog.InsertTransactionLog(transactionLogTRY);

                                    Console.WriteLine("\nWithdraw unsuccessful.");

                                    break;

                                default:

                                    Console.WriteLine("Invalid Choice!");
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Insufficient balance!");
                }
            }
            else
            {
                Console.WriteLine($"\nThe account number you entered does not belong to our bank. If you continue the transaction, a transaction fee of {transactionFee} will be charged.");
                Console.WriteLine("\nDo you want to continue? 1-) Yes, 2-) No");
                Console.Write(">>> ");
                string? ch = Console.ReadLine();

                switch (ch)
                {
                    case "1":

                        // Log the successful deposit
                        DTOTransactionLog transactionLogs = new DTOTransactionLog
                        {
                            TargetAccountNumber = accountInfo.AccountNo,
                            TransactionType = EnumTransactionType.WithdrawToAnother,
                            TransactionStatus = EnumTransactionStatus.Success,
                            TransferAmount = balance,
                            Timestamp = DateTime.Now,
                            TransactionFee = transactionFee
                        };

                        eTransactionLog.InsertTransactionLog(transactionLogs);

                        Console.WriteLine("\nWithdraw successful.");

                        break;

                    case "2":

                        // Log the failed deposit
                        DTOTransactionLog transactionLogTRY = new DTOTransactionLog
                        {
                            TargetAccountNumber = accountInfo.AccountNo,
                            TransactionType = EnumTransactionType.WithdrawToAnother,
                            TransactionStatus = EnumTransactionStatus.Failed,
                            TransferAmount = balance,
                            Timestamp = DateTime.Now,
                        };

                        eTransactionLog.InsertTransactionLog(transactionLogTRY);

                        Console.WriteLine("\nWithdraw unsuccessful.");

                        break;

                    default:

                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }

        /// <summary>
        /// BOA process
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="transfer"></param>
        public void TransferBetweenAccounts(int customerId, DTOTransfer transfer)
        {
            try
            {
                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                {
                    CustomerId = customerId
                };
                List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccount);

                if (transfer.SourceAccountIndex >= 0 && transfer.SourceAccountIndex < accountInfos.Count && transfer.TargetAccountIndex >= 0 && transfer.TargetAccountIndex < accountInfos.Count)
                {
                    long sourceAccountNo = accountInfos[transfer.SourceAccountIndex].AccountNo;
                    long targetAccountNo = accountInfos[transfer.TargetAccountIndex].AccountNo;
                    int sourceCurrencyType = accountInfos[transfer.SourceAccountIndex].CurrencyType;
                    int targetCurrencyType = accountInfos[transfer.TargetAccountIndex].CurrencyType;
                    decimal sourceAccountBalance = accountInfos[transfer.SourceAccountIndex].Balance;
                    decimal targetAccountBalance = accountInfos[transfer.TargetAccountIndex].Balance;

                    DTOCurrencyMovement currencyMovement = new DTOCurrencyMovement
                    {
                        SourceAccountNo = sourceAccountNo,
                        TargetAccountNo = targetAccountNo,
                        SourceCurrencyType = sourceCurrencyType,
                        TargetCurrencyType = targetCurrencyType,
                        Amount = transfer.Amount
                    };

                    if (currencyMovement.SourceCurrencyType == currencyMovement.TargetCurrencyType)
                    {
                        if (sourceAccountNo != targetAccountNo)
                        {
                            DTOAccountInfo accountInfo = new DTOAccountInfo()
                            {
                                AccountNo = sourceAccountNo
                            };

                            bool successfulTransfer = account.TransferBetweenAccounts(currencyMovement, accountInfo);
                            if (successfulTransfer)
                            {
                                Console.WriteLine("Transfer Between Accounts transaction has been done successfuly.");
                            }
                            else
                            {
                                // Log the failed transfer
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    SourceAccountNumber = sourceAccountNo,
                                    TargetAccountNumber = targetAccountNo,
                                    TransactionType = EnumTransactionType.BOATransfer,
                                    TransactionStatus = EnumTransactionStatus.Failed,
                                    TransferAmount = transfer.Amount,
                                    SourceOldBalance = sourceAccountBalance,
                                    SourceNewBalance = sourceAccountBalance,
                                    TargetOldBalance = targetAccountBalance,
                                    TargetNewBalance = targetAccountBalance,
                                    Timestamp = DateTime.Now,
                                    SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)sourceCurrencyType,
                                    TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)targetCurrencyType
                                };
                                eTransactionLog.InsertTransactionLog(transactionLog);

                                Console.WriteLine("Transfer Between Accounts transaction could not be done. Please try again.");
                            }
                        }
                        else
                        {
                            // Log the failed transfer
                            DTOTransactionLog transactionLog = new DTOTransactionLog
                            {
                                SourceAccountNumber = sourceAccountNo,
                                TargetAccountNumber = targetAccountNo,
                                TransactionType = EnumTransactionType.BOATransfer,
                                TransactionStatus = EnumTransactionStatus.Failed,
                                TransferAmount = transfer.Amount,
                                SourceOldBalance = sourceAccountBalance,
                                SourceNewBalance = sourceAccountBalance,
                                TargetOldBalance = targetAccountBalance,
                                TargetNewBalance = targetAccountBalance,
                                Timestamp = DateTime.Now,
                                SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)sourceCurrencyType,
                                TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)targetCurrencyType
                            };
                            eTransactionLog.InsertTransactionLog(transactionLog);

                            Console.WriteLine("Source account and target account can not be same. The transfer could not be performed.");
                        }
                    }
                    else
                    {
                        // Log the failed transfer
                        DTOTransactionLog transactionLog = new DTOTransactionLog
                        {
                            SourceAccountNumber = sourceAccountNo,
                            TargetAccountNumber = targetAccountNo,
                            TransactionType = EnumTransactionType.BOATransfer,
                            TransactionStatus = EnumTransactionStatus.Failed,
                            TransferAmount = transfer.Amount,
                            SourceOldBalance = sourceAccountBalance,
                            SourceNewBalance = sourceAccountBalance,
                            TargetOldBalance = targetAccountBalance,
                            TargetNewBalance = targetAccountBalance,
                            Timestamp = DateTime.Now,
                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)sourceCurrencyType,
                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)targetCurrencyType

                        };
                        eTransactionLog.InsertTransactionLog(transactionLog);

                        Console.WriteLine("Source account and target account currency types do not match. The transfer could not be performed.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid account index. Try again.");
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
        /// Havale/EFT process
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="transfer"></param>
        public void HavaleEFT(int customerId, DTOTransfer transfer)
        {
            try
            {
                DTOAccountInfo dTOAccount1 = new DTOAccountInfo()
                {
                    AccountNo = transfer.TargetAccountNo
                };

                DTOAccountInfo targetAccountInfo = eAccount.ReadAccountInfo(dTOAccount1);
                if (targetAccountInfo.CustomerId != 0)
                {
                    decimal transactionFee = eTransactionFee.ReadTransactionFee(EnumTransactionFeeType.Havale);

                    int targetCurrencyType = targetAccountInfo.CurrencyType;

                    DTOAccountInfo dTOAccount = new DTOAccountInfo()
                    {
                        CustomerId = customerId
                    };
                    List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccount);

                    if (transfer.SourceAccountIndex >= 0 && transfer.SourceAccountIndex < accountInfos.Count)
                    {
                        long sourceAccountNo = accountInfos[transfer.SourceAccountIndex].AccountNo;
                        int sourceCurrencyType = accountInfos[transfer.SourceAccountIndex].CurrencyType;

                        DTOCurrencyMovement currencyMovement = new DTOCurrencyMovement
                        {
                            SourceAccountNo = sourceAccountNo,
                            TargetAccountNo = transfer.TargetAccountNo,
                            SourceCurrencyType = sourceCurrencyType,
                            TargetCurrencyType = targetCurrencyType,
                            Amount = transfer.Amount,
                            Fee = transactionFee,
                            Type = EnumTransactionType.Havale,
                        };

                        if (sourceCurrencyType == targetCurrencyType)
                        {
                            bool isOwnAccount = IsOwnAccount(accountInfos, transfer.TargetAccountNo);
                            if (isOwnAccount)
                            {
                                Console.WriteLine("The target account is your own account. Remittance cannot be performed.");
                            }
                            else
                            {
                                DTOAccountInfo accountInfo = new DTOAccountInfo()
                                {
                                    AccountNo = sourceAccountNo
                                };

                                bool successfulTransfer = account.HavaleEFT(currencyMovement, accountInfo);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Source account and target account currency types do not match. Transfer could not be performed.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid account index. Try again.");
                    }
                }
                else //EFT
                {
                    decimal transactionFee = eTransactionFee.ReadTransactionFee(EnumTransactionFeeType.EFT);

                    DTOAccountInfo dTOAccount = new DTOAccountInfo()
                    {
                        CustomerId = customerId
                    };
                    List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccount);

                    if (transfer.SourceAccountIndex >= 0 && transfer.SourceAccountIndex < accountInfos.Count)
                    {
                        long sourceAccountNo = accountInfos[transfer.SourceAccountIndex].AccountNo;
                        int sourceCurrencyType = accountInfos[transfer.SourceAccountIndex].CurrencyType;

                        DTOCurrencyMovement currencyMovement = new DTOCurrencyMovement
                        {
                            SourceAccountNo = sourceAccountNo,
                            TargetAccountNo = transfer.TargetAccountNo,
                            Amount = transfer.Amount,
                            Fee = transactionFee,
                            Type = EnumTransactionType.EFT
                        };

                        bool isOwnAccount = IsOwnAccount(accountInfos, transfer.TargetAccountNo);
                        if (isOwnAccount)
                        {
                            Console.WriteLine("The target account is your own account. Havale/EFT cannot be performed.");
                        }
                        else
                        {
                            DTOAccountInfo accountInfo = new DTOAccountInfo()
                            {
                                AccountNo = sourceAccountNo
                            };

                            bool successfulTransfer = account.HavaleEFT(currencyMovement, accountInfo);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid account index. Try again.");
                    }
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
        /// Exchange buying process
        /// </summary>
        /// <param name="dTOExchange"></param>
        public void ExchangeBuying(DTOExchange dTOExchange)
        {
            try
            {
                decimal transactionFee = eTransactionFee.ReadTransactionFee(EnumTransactionFeeType.CurrencyBuyingProfitMargin);
                decimal kmvRate = eTransactionFee.ReadTransactionFee(EnumTransactionFeeType.KMV);
                decimal kmv = (kmvRate * dTOExchange.Amount) * dTOExchange.ExchangeRate;
                decimal money = dTOExchange.Amount * dTOExchange.ExchangeRate - transactionFee;
                decimal sourceNewBalance = dTOExchange.SourceAccountBalance - money;
                decimal targetNewBalance = dTOExchange.TargetAccountBalance + dTOExchange.Amount;
                if (dTOExchange.SourceAccountBalance >= money)
                {
                    DTOAccountInfo sourceUpdate = new DTOAccountInfo()
                    {
                        AccountNo = dTOExchange.SourceAccountNo,
                        Balance = sourceNewBalance,
                        AccountName = dTOExchange.SourceAccountName
                    };

                    eAccount.UpdateAccountInfo(sourceUpdate);

                    DTOAccountInfo targetUpdate = new DTOAccountInfo()
                    {
                        AccountNo = dTOExchange.TargetAccountNo,
                        Balance = targetNewBalance,
                        AccountName = dTOExchange.TargetAccountName
                    };

                    eAccount.UpdateAccountInfo(targetUpdate);

                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        SourceAccountNumber = dTOExchange.SourceAccountNo,
                        TargetAccountNumber = dTOExchange.TargetAccountNo,
                        TransactionType = EnumTransactionType.CurrenySelling,
                        TransactionStatus = EnumTransactionStatus.Success,
                        TransferAmount = dTOExchange.Amount,
                        CurrencyRate = dTOExchange.ExchangeRate,
                        SourceOldBalance = dTOExchange.SourceAccountBalance,
                        SourceNewBalance = sourceNewBalance,
                        TargetOldBalance = dTOExchange.TargetAccountBalance,
                        TargetNewBalance = targetNewBalance,
                        Timestamp = DateTime.Now,
                        TransactionFee = transactionFee,
                        KMV = kmv,
                        SourceCurrencyType = EnumCurrencyTypes.CurrencyTypes.TRY,
                        TargetCurrencyType = dTOExchange.CurrencyType
                    };
                    eTransactionLog.InsertTransactionLog(transactionLog);

                    Console.WriteLine($"\n{dTOExchange.CurrencyType} buying successful.");
                }
                else
                {
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        SourceAccountNumber = dTOExchange.SourceAccountNo,
                        TargetAccountNumber = dTOExchange.TargetAccountNo,
                        TransactionType = EnumTransactionType.CurrenySelling,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        TransferAmount = dTOExchange.Amount,
                        CurrencyRate = dTOExchange.ExchangeRate,
                        SourceOldBalance = dTOExchange.SourceAccountBalance,
                        SourceNewBalance = dTOExchange.SourceAccountBalance,
                        TargetOldBalance = dTOExchange.TargetAccountBalance,
                        TargetNewBalance = dTOExchange.TargetAccountBalance,
                        Timestamp = DateTime.Now,
                        SourceCurrencyType = EnumCurrencyTypes.CurrencyTypes.TRY,
                        TargetCurrencyType = dTOExchange.CurrencyType
                    };
                    eTransactionLog.InsertTransactionLog(transactionLog);

                    Console.WriteLine("Insufficient balance.");
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
        /// Exchange selling process
        /// </summary>
        /// <param name="dTOExchange"></param>
        public void ExchangeSelling(DTOExchange dTOExchange)
        {
            try
            {
                decimal transactionFee = eTransactionFee.ReadTransactionFee(EnumTransactionFeeType.CurrencySellingProfitMargin);
                decimal money = dTOExchange.Amount * dTOExchange.ExchangeRate - transactionFee;
                decimal sourceNewBalance = dTOExchange.SourceAccountBalance - dTOExchange.Amount;
                decimal targetNewBalance = dTOExchange.TargetAccountBalance + money;
                if (dTOExchange.SourceAccountBalance >= dTOExchange.Amount)
                {
                    DTOAccountInfo sourceUpdate = new DTOAccountInfo()
                    {
                        AccountNo = dTOExchange.SourceAccountNo,
                        Balance = sourceNewBalance,
                        AccountName = dTOExchange.SourceAccountName
                    };

                    eAccount.UpdateAccountInfo(sourceUpdate);

                    DTOAccountInfo targetUpdate = new DTOAccountInfo()
                    {
                        AccountNo = dTOExchange.TargetAccountNo,
                        Balance = targetNewBalance,
                        AccountName = dTOExchange.TargetAccountName
                    };

                    eAccount.UpdateAccountInfo(targetUpdate);

                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        SourceAccountNumber = dTOExchange.SourceAccountNo,
                        TargetAccountNumber = dTOExchange.TargetAccountNo,
                        TransactionType = EnumTransactionType.CurrencyBuying,
                        TransactionStatus = EnumTransactionStatus.Success,
                        TransferAmount = dTOExchange.Amount,
                        CurrencyRate = dTOExchange.ExchangeRate,
                        SourceOldBalance = dTOExchange.SourceAccountBalance,
                        SourceNewBalance = sourceNewBalance,
                        TargetOldBalance = dTOExchange.TargetAccountBalance,
                        TargetNewBalance = targetNewBalance,
                        Timestamp = DateTime.Now,
                        TransactionFee = transactionFee,
                        SourceCurrencyType = dTOExchange.CurrencyType,
                        TargetCurrencyType = EnumCurrencyTypes.CurrencyTypes.TRY
                    };

                    eTransactionLog.InsertTransactionLog(transactionLog);

                    Console.WriteLine($"\n{dTOExchange.CurrencyType} selling successful.");
                }
                else
                {
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        SourceAccountNumber = dTOExchange.SourceAccountNo,
                        TargetAccountNumber = dTOExchange.TargetAccountNo,
                        TransactionType = EnumTransactionType.CurrencyBuying,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        TransferAmount = dTOExchange.Amount,
                        CurrencyRate = dTOExchange.ExchangeRate,
                        SourceOldBalance = dTOExchange.SourceAccountBalance,
                        SourceNewBalance = dTOExchange.SourceAccountBalance,
                        TargetOldBalance = dTOExchange.TargetAccountBalance,
                        TargetNewBalance = dTOExchange.TargetAccountBalance,
                        Timestamp = DateTime.Now,
                        SourceCurrencyType = dTOExchange.CurrencyType,
                        TargetCurrencyType = EnumCurrencyTypes.CurrencyTypes.TRY
                    };
                    eTransactionLog.InsertTransactionLog(transactionLog);

                    Console.WriteLine("Insufficient balance.");
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
        /// Control for money transfers
        /// </summary>
        /// <param name="accountInfos"></param>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        private bool IsOwnAccount(List<DTOAccountInfo> accountInfos, long accountNo)
        {
            foreach (DTOAccountInfo accountInfo in accountInfos)
            {
                if (accountInfo.AccountNo == accountNo)
                {
                    return true;
                }
            }
            return false;
        }
    }
}