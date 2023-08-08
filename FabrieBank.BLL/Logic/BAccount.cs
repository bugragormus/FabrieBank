using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;
using FabrieBank.DAL;
using FabrieBank.DAL.Entity;

namespace FabrieBank.BLL.Logic
{
    public class BAccount
    {

        private DataAccessLayer dataAccessLayer;
        private EAccountInfo eAccount;

        public BAccount()
        {
            eAccount = new EAccountInfo();
            dataAccessLayer = new DataAccessLayer();
        }

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

            EAccountInfo accInfoDB = new EAccountInfo();
            accInfoDB.ReadListAccountInfo(dTOAccount);
            if (accountInfos.Count == 0)
            {
                Console.WriteLine("yok");
            }

        }

        /// <summary>
        /// Hesap Silme
        /// </summary>
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
        /// Prints customers accounts
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
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

        /// <summary>
        /// BOA B.L.
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
                    // Log the failed transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        AccountNumber = movement.SourceAccountNo,
                        TargetAccountNumber = movement.TargetAccountNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        Amount = movement.Amount,
                        OldBalance = oldBalance,
                        NewBalance = oldBalance,
                        Timestamp = DateTime.Now
                    };
                    dataAccessLayer.LogTransaction(transactionLog);
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
                        AccountNumber = movement.SourceAccountNo,
                        TargetAccountNumber = movement.TargetAccountNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Success,
                        Amount = movement.Amount,
                        OldBalance = oldBalance,
                        NewBalance = newBalance,
                        Timestamp = DateTime.Now
                    };

                    dataAccessLayer.LogTransaction(transactionLog);

                    return true;
                }
                else
                {
                    // Log the failed transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        AccountNumber = movement.SourceAccountNo,
                        TargetAccountNumber = movement.TargetAccountNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        Amount = movement.Amount,
                        OldBalance = oldBalance,
                        NewBalance = oldBalance,
                        Timestamp = DateTime.Now
                    };

                    DTOAccountInfo updateSource = new DTOAccountInfo()
                    {
                        AccountNo = movement.SourceAccountNo,
                        Balance = oldBalance,
                        AccountName = accountInfo.AccountName
                    };

                    dataAccessLayer.LogTransaction(transactionLog);
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
                    AccountNumber = movement.SourceAccountNo,
                    TargetAccountNumber = movement.TargetAccountNo,
                    TransactionType = EnumTransactionType.BOATransfer,
                    TransactionStatus = EnumTransactionStatus.Failed,
                    Amount = movement.Amount,
                    OldBalance = accountInfo.Balance,
                    NewBalance = accountInfo.Balance,
                    Timestamp = DateTime.Now
                };
                dataAccessLayer.LogTransaction(transactionLog);
                Console.WriteLine("\nThe source account was not found.");
                return false;
            }
        }

        /// <summary>
        /// Havale/EFT B.L.
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

                if (oldBalance < movement.Amount)
                {
                    Console.WriteLine("\nInsufficient balance. Transfer failed.");

                    // Log the failed transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        AccountNumber = movement.SourceAccountNo,
                        TargetAccountNumber = movement.TargetAccountNo,
                        TransactionType = movement.Type,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        Amount = movement.Amount,
                        OldBalance = oldBalance,
                        NewBalance = oldBalance,
                        Timestamp = DateTime.Now
                    };
                    dataAccessLayer.LogTransaction(transactionLog);
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
                        AccountNumber = movement.SourceAccountNo,
                        TargetAccountNumber = movement.TargetAccountNo,
                        TransactionType = movement.Type,
                        TransactionStatus = EnumTransactionStatus.Success,
                        Amount = movement.Amount,
                        OldBalance = oldBalance,
                        NewBalance = newBalance,
                        Timestamp = DateTime.Now,
                        TransactionFee = movement.Fee
                    };

                    dataAccessLayer.LogTransaction(transactionLog);
                    Console.WriteLine("Havale Transaction Successfully Performed.");

                    return true;
                }
                else
                {
                    Console.WriteLine("EFT Transaction Successfully Performed.");

                    // Log the successful transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        AccountNumber = movement.SourceAccountNo,
                        TargetAccountNumber = movement.TargetAccountNo,
                        TransactionType = movement.Type,
                        TransactionStatus = EnumTransactionStatus.Success,
                        Amount = movement.Amount,
                        OldBalance = oldBalance,
                        NewBalance = newBalance,
                        Timestamp = DateTime.Now,
                        TransactionFee = movement.Fee
                    };

                    dataAccessLayer.LogTransaction(transactionLog);

                    return true;

                    //// Log the failed transfer
                    //DTOTransactionLog transactionLog = new DTOTransactionLog
                    //{
                    //    AccountNumber = hareket.KaynakHesapNo,
                    //    TargetAccountNumber = hareket.HedefHesapNo,
                    //    TransactionType = EnumTransactionType.BOATransfer,
                    //    TransactionStatus = EnumTransactionStatus.Failed,
                    //    Amount = hareket.Miktar,
                    //    OldBalance = eskiBakiye,
                    //    NewBalance = eskiBakiye,
                    //    Timestamp = DateTime.Now
                    //};

                    //DTOAccountInfo updateKaynak = new DTOAccountInfo()
                    //{
                    //    HesapNo = hareket.KaynakHesapNo,
                    //    Bakiye = eskiBakiye,
                    //    HesapAdi = accountInfo.HesapAdi
                    //};

                    //dataAccessLayer.LogTransaction(transactionLog);
                    //eAccount.UpdateAccountInfo(updateKaynak);

                    //return false;
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