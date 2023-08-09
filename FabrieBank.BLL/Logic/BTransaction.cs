using System.Security.Principal;
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

        public BTransaction()
        {
            eAccount = new EAccountInfo();
            account = new BAccount();
            dataAccessLayer = new DataAccessLayer();
        }

        /// <summary>
        /// Deposit B.L.
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <param name="balance"></param>
        public void Deposit(DTOAccountInfo accountInfo, decimal balance)
        {
            accountInfo = eAccount.ReadAccountInfo(accountInfo);
            if (accountInfo != null)
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
                    Timestamp = DateTime.Now
                };

                dataAccessLayer.LogTransaction(transactionLog);

                Console.WriteLine("\nDeposit successful.");
                Console.WriteLine($"Old balance: {oldBalance}");
                Console.WriteLine($"New balance: {newBalance}");
            }
            else
            {
                // Log the failed deposit
                DTOTransactionLog transactionLog = new DTOTransactionLog
                {
                    TargetAccountNumber = accountInfo.AccountNo,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Failed,
                    TransferAmount = balance,
                    TargetOldBalance = accountInfo.Balance,
                    TargetNewBalance = accountInfo.Balance,
                    Timestamp = DateTime.Now
                };

                dataAccessLayer.LogTransaction(transactionLog);

                Console.WriteLine("\nDeposit unsuccessful.");
            }
        }

        /// <summary>
        /// Withdraw B.L.
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <param name="balance"></param>
        public void Withdraw(DTOAccountInfo accountInfo, decimal balance)
        {
            accountInfo = eAccount.ReadAccountInfo(accountInfo);
            if (accountInfo != null)
            {
                if (accountInfo.Balance >= balance)
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
                        Timestamp = DateTime.Now
                    };

                    dataAccessLayer.LogTransaction(transactionLog);

                    Console.WriteLine("\nWithdraw successful");
                    Console.WriteLine($"Old balance: {oldBalance}");
                    Console.WriteLine($"New Balance: {newBalance}");
                }
                else
                {
                    // Log the failed deposit
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        TargetAccountNumber = accountInfo.AccountNo,
                        TransactionType = EnumTransactionType.Withdrawal,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        TransferAmount = balance,
                        TargetOldBalance = accountInfo.Balance,
                        TargetNewBalance = accountInfo.Balance,
                        Timestamp = DateTime.Now
                    };
                    dataAccessLayer.LogTransaction(transactionLog);

                    Console.WriteLine("Insufficient balance.");
                }
            }
            else
            {
                // Log the failed deposit
                DTOTransactionLog transactionLog = new DTOTransactionLog
                {
                    TargetAccountNumber = accountInfo.AccountNo,
                    TransactionType = EnumTransactionType.Withdrawal,
                    TransactionStatus = EnumTransactionStatus.Failed,
                    TransferAmount = balance,
                    TargetOldBalance = accountInfo.Balance,
                    TargetNewBalance = accountInfo.Balance,
                    Timestamp = DateTime.Now
                };

                dataAccessLayer.LogTransaction(transactionLog);

                Console.WriteLine("\nWithdraw unsuccessful.");
            }
        }

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
                                Timestamp = DateTime.Now
                            };
                            dataAccessLayer.LogTransaction(transactionLog);

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
                            Timestamp = DateTime.Now
                        };
                        dataAccessLayer.LogTransaction(transactionLog);

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
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

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
                    decimal transactionFee = dataAccessLayer.GetTransactionFee(EnumTransactionFeeType.Havale);

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
                    decimal transactionFee = dataAccessLayer.GetTransactionFee(EnumTransactionFeeType.EFT);

                    DTOAccountInfo dTOAccount = new DTOAccountInfo()
                    {
                        CustomerId = customerId
                    };
                    List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccount);

                    if (transfer.SourceAccountIndex >= 0 && transfer.SourceAccountIndex< accountInfos.Count)
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
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

        public void ExchangeBuying(DTOExchange dTOExchange)
        {
            try
            {
                decimal transactionFee = dataAccessLayer.GetTransactionFee(EnumTransactionFeeType.CurrencyFee);
                decimal money = dTOExchange.Amount * (dTOExchange.ExchangeRate + (dTOExchange.ExchangeRate * 0.05M));
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
                        TransactionType = EnumTransactionType.CurrencyBuying,
                        TransactionStatus = EnumTransactionStatus.Success,
                        TransferAmount = dTOExchange.Amount,
                        CurrencyRate = dTOExchange.ExchangeRate,
                        SourceOldBalance = dTOExchange.SourceAccountBalance,
                        SourceNewBalance = sourceNewBalance,
                        TargetOldBalance = dTOExchange.TargetAccountBalance,
                        TargetNewBalance = targetNewBalance,
                        Timestamp = DateTime.Now,
                        TransactionFee = transactionFee
                    };

                    dataAccessLayer.LogTransaction(transactionLog);

                    Console.WriteLine($"\n{dTOExchange.CurrencyType} buying successful.");
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
                        Timestamp = DateTime.Now
                    };
                    dataAccessLayer.LogTransaction(transactionLog);

                    Console.WriteLine("Insufficient balance.");
                }

            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

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