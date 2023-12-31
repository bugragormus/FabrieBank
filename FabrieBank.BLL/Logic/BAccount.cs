﻿using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;
using FabrieBank.DAL.Entity;
using System.Reflection;

namespace FabrieBank.BLL.Logic
{
    public class BAccount
    {
        private ETransactionLog eTransaction;
        private EAccountInfo eAccount;

        public BAccount()
        {
            eTransaction = new ETransactionLog();
            eAccount = new EAccountInfo();
        }

        /// <summary>
        /// Creating account process
        /// </summary>
        /// <param name="accountInfo"></param>
        public void CreateAccount(DTOAccountInfo accountInfo)
        {
            _ = eAccount.InsertAccountInfo(accountInfo);

            int numericValue = accountInfo.CurrencyType;
            string currencyName = Enum.GetName(typeof(EnumCurrencyTypes.CurrencyTypes), numericValue);

            if (accountInfo.AccountName != "")
            {
                Console.WriteLine($"\n'{accountInfo.AccountName}' named new '{currencyName}' account has been created.\n");
            }
            else
            {
                Console.WriteLine($"\nNew '{currencyName}' account has been created.\n");
            }
        }

        /// <summary>
        /// Account listing process
        /// </summary>
        /// <param name="customer"></param>
        public void AccountList(DTOCustomer customer)
        {
            EAccountInfo eAccount1 = new EAccountInfo();
            DTOAccountInfo dTOAccount = new DTOAccountInfo()
            {
                CustomerId = customer.CustomerId,
            };
            List<DTOAccountInfo> accountInfos = eAccount1.ReadListAccountInfo(dTOAccount);

            foreach (DTOAccountInfo accountInfo in accountInfos)
            {
                Console.WriteLine($"Account No: {accountInfo.AccountNo}");
                Console.WriteLine($"Balance: {accountInfo.Balance}");
                Console.WriteLine($"Currency Type: {accountInfo.CurrencyType}");
                Console.WriteLine($"Account Name: {accountInfo.AccountName}");
                Console.WriteLine("==============================\n");
            }
            if (accountInfos.Count == 0)
            {
                Console.WriteLine("yok");
            }
        }

        /// <summary>
        /// Account deleting process
        /// </summary>
        /// <param name="customer"></param>
        public void DeleteAccount(DTOCustomer customer)
        {
            DTOAccountInfo dTOAccounts = new DTOAccountInfo()
            {
                CustomerId = customer.CustomerId
            };
            List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccounts);

            Console.WriteLine("\nWhich account would you like to delete?");
            PrintAccountList(accountInfos);

            Console.Write("Account Index: ");
            int deletedAccIndex = int.Parse(Console.ReadLine());

            if (deletedAccIndex >= 0 && deletedAccIndex < accountInfos.Count)
            {
                long deletedAccNo = accountInfos[deletedAccIndex].AccountNo;

                foreach (DTOAccountInfo accountInf in accountInfos)
                {
                    if (accountInf.AccountNo == deletedAccNo)
                    {
                        DTOAccountInfo dTOAccountInfo = new DTOAccountInfo()
                        {
                            AccountNo = deletedAccNo
                        };
                        _ = eAccount.DeleteAccountInfo(dTOAccountInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Printing account list process
        /// </summary>
        /// <param name="accountInfos"></param>
        public void PrintAccountList(List<DTOAccountInfo> accountInfos)
        {
            try
            {
                Console.WriteLine("Your Accounts:\n");
                for (int i = 0; i < accountInfos.Count; i++)
                {
                    Console.WriteLine($"[{i}] Account No: {accountInfos[i].AccountNo}");
                    Console.WriteLine($"Balance: {accountInfos[i].Balance}");
                    Console.WriteLine($"Currency Type: {accountInfos[i].CurrencyType}");
                    Console.WriteLine("==============================\n");
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
        /// Money transfer between same persons accounts
        /// </summary>
        /// <param name="movement"></param>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        public bool TransferBetweenAccounts(DTOCurrencyMovement movement, DTOAccountInfo accountInfo)
        {
            accountInfo = eAccount.ReadAccountInfo(accountInfo);
            if (accountInfo != null)
            {
                decimal oldBalance = accountInfo.Balance;
                decimal newBalance = oldBalance - movement.Amount;

                if (oldBalance < movement.Amount)
                {
                    DTOAccountInfo dTOAccountss = new DTOAccountInfo()
                    {
                        AccountNo = movement.TargetAccountNo
                    };

                    accountInfo = eAccount.ReadAccountInfo(dTOAccountss);

                    // Log the failed transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        SourceAccountNumber = movement.SourceAccountNo,
                        TargetAccountNumber = movement.TargetAccountNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        TransferAmount = movement.Amount,
                        SourceOldBalance = oldBalance,
                        SourceNewBalance = oldBalance,
                        TargetOldBalance = accountInfo.Balance,
                        TargetNewBalance = accountInfo.Balance,
                        Timestamp = DateTime.Now,
                        SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.SourceCurrencyType,
                        TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.TargetCurrencyType
                    };
                    eTransaction.InsertTransactionLog(transactionLog);
                    Console.WriteLine("\nInsufficient balance. The transfer could not be performed.");
                    return false;
                }
                else
                {
                    DTOAccountInfo updateSource = new DTOAccountInfo()
                    {
                        AccountNo = movement.SourceAccountNo,
                        Balance = newBalance,
                        AccountName = accountInfo.AccountName
                    };
                    eAccount.UpdateAccountInfo(updateSource);
                }

                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                {
                    AccountNo = movement.TargetAccountNo
                };

                accountInfo = eAccount.ReadAccountInfo(dTOAccount);
                if (dTOAccount != null)
                {
                    DTOAccountInfo updateTarget = new DTOAccountInfo()
                    {
                        AccountNo = movement.TargetAccountNo,
                        Balance = accountInfo.Balance + movement.Amount,
                        AccountName = accountInfo.AccountName
                    };

                    eAccount.UpdateAccountInfo(updateTarget);

                    // Log the successful transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        SourceAccountNumber = movement.SourceAccountNo,
                        TargetAccountNumber = movement.TargetAccountNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Success,
                        TransferAmount = movement.Amount,
                        SourceOldBalance = oldBalance,
                        SourceNewBalance = newBalance,
                        TargetOldBalance = dTOAccount.Balance,
                        TargetNewBalance = dTOAccount.Balance + movement.Amount,
                        Timestamp = DateTime.Now,
                        SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.SourceCurrencyType,
                        TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.TargetCurrencyType
                    };

                    eTransaction.InsertTransactionLog(transactionLog);

                    return true;
                }
                else
                {
                    // Log the failed transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        SourceAccountNumber = movement.SourceAccountNo,
                        TargetAccountNumber = movement.TargetAccountNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        TransferAmount = movement.Amount,
                        SourceOldBalance = oldBalance,
                        SourceNewBalance = oldBalance,
                        TargetOldBalance = dTOAccount.Balance,
                        TargetNewBalance = dTOAccount.Balance,
                        Timestamp = DateTime.Now,
                        SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.SourceCurrencyType,
                        TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.TargetCurrencyType
                    };

                    DTOAccountInfo updateSource = new DTOAccountInfo()
                    {
                        AccountNo = movement.SourceAccountNo,
                        Balance = oldBalance,
                        AccountName = accountInfo.AccountName
                    };

                    eTransaction.InsertTransactionLog(transactionLog);
                    eAccount.UpdateAccountInfo(updateSource);

                    Console.WriteLine("\nThe target account was not found.");
                    return false;
                }
            }
            else
            {
                // Log the failed transfer
                DTOTransactionLog transactionLog = new DTOTransactionLog
                {
                    SourceAccountNumber = movement.SourceAccountNo,
                    TargetAccountNumber = movement.TargetAccountNo,
                    TransactionType = EnumTransactionType.BOATransfer,
                    TransactionStatus = EnumTransactionStatus.Failed,
                    TransferAmount = movement.Amount,
                    SourceOldBalance = accountInfo.Balance,
                    SourceNewBalance = accountInfo.Balance,
                    Timestamp = DateTime.Now,
                    SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.SourceCurrencyType,
                    TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.TargetCurrencyType
                };
                eTransaction.InsertTransactionLog(transactionLog);
                Console.WriteLine("\nThe source account was not found.");
                return false;
            }
        }

        /// <summary>
        /// Havale/EFT process
        /// </summary>
        /// <param name="movement"></param>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        public bool HavaleEFT(DTOCurrencyMovement movement, DTOAccountInfo accountInfo)
        {
            accountInfo = eAccount.ReadAccountInfo(accountInfo);
            if (accountInfo != null)
            {
                decimal oldBalance = accountInfo.Balance;
                decimal newBalance = oldBalance - movement.Amount - movement.Fee;

                if (oldBalance < movement.Amount + movement.Fee)
                {
                    Console.WriteLine("\nInsufficient balance. Transfer failed.");

                    DTOAccountInfo dTOAccountsss = new DTOAccountInfo()
                    {
                        AccountNo = movement.TargetAccountNo
                    };

                    accountInfo = eAccount.ReadAccountInfo(dTOAccountsss);

                    // Log the failed transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        SourceAccountNumber = movement.SourceAccountNo,
                        TargetAccountNumber = movement.TargetAccountNo,
                        TransactionType = movement.Type,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        TransferAmount = movement.Amount,
                        SourceOldBalance = oldBalance,
                        SourceNewBalance = oldBalance,
                        TargetOldBalance = accountInfo.Balance,
                        TargetNewBalance = accountInfo.Balance,
                        Timestamp = DateTime.Now,
                        SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.SourceCurrencyType,
                        TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.TargetCurrencyType
                    };
                    eTransaction.InsertTransactionLog(transactionLog);
                }
                else
                {
                    DTOAccountInfo updateSource = new DTOAccountInfo()
                    {
                        AccountNo = movement.SourceAccountNo,
                        Balance = newBalance,
                        AccountName = accountInfo.AccountName
                    };
                    eAccount.UpdateAccountInfo(updateSource);

                    DTOAccountInfo dTOAccount = new DTOAccountInfo()
                    {
                        AccountNo = movement.TargetAccountNo
                    };

                    accountInfo = eAccount.ReadAccountInfo(dTOAccount);
                    if (accountInfo.CustomerId != 0)
                    {
                        DTOAccountInfo updateTarget = new DTOAccountInfo()
                        {
                            AccountNo = movement.TargetAccountNo,
                            Balance = accountInfo.Balance + movement.Amount,
                            AccountName = accountInfo.AccountName
                        };

                        eAccount.UpdateAccountInfo(updateTarget);

                        // Log the successful transfer
                        DTOTransactionLog transactionLog = new DTOTransactionLog
                        {
                            SourceAccountNumber = movement.SourceAccountNo,
                            TargetAccountNumber = movement.TargetAccountNo,
                            TransactionType = movement.Type,
                            TransactionStatus = EnumTransactionStatus.Success,
                            TransferAmount = movement.Amount,
                            SourceOldBalance = oldBalance,
                            SourceNewBalance = newBalance,
                            TargetOldBalance = accountInfo.Balance,
                            TargetNewBalance = accountInfo.Balance + movement.Amount,
                            Timestamp = DateTime.Now,
                            TransactionFee = movement.Fee,
                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.SourceCurrencyType,
                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.TargetCurrencyType
                        };

                        eTransaction.InsertTransactionLog(transactionLog);
                        Console.WriteLine("Havale Transaction Successfully Performed.");

                        return true;
                    }
                    else
                    {
                        Console.WriteLine("EFT Transaction Successfully Performed.");

                        // Log the successful transfer
                        DTOTransactionLog transactionLog = new DTOTransactionLog
                        {
                            SourceAccountNumber = movement.SourceAccountNo,
                            TargetAccountNumber = movement.TargetAccountNo,
                            TransactionType = movement.Type,
                            TransactionStatus = EnumTransactionStatus.Success,
                            TransferAmount = movement.Amount,
                            SourceOldBalance = oldBalance,
                            SourceNewBalance = newBalance,
                            Timestamp = DateTime.Now,
                            TransactionFee = movement.Fee,
                            SourceCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.SourceCurrencyType,
                            TargetCurrencyType = (EnumCurrencyTypes.CurrencyTypes)movement.TargetCurrencyType
                        };

                        eTransaction.InsertTransactionLog(transactionLog);

                        return true;
                    }
                }
            }
            else
            {
                Console.WriteLine("The source account number could not be found.");
            }
            return true;
        }
    }
}