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

        //public string GetCurrencyType(int currencyType)
        //{
        //    switch (currencyType)
        //    {
        //        case 1:
        //            return "Turkish Lira (TRY)";
        //        case 2:
        //            return "American Dollar (USD)";
        //        case 3:
        //            return "Euro (EUR)";
        //        case 4:
        //            return "Gram Altın (GBP)";
        //        case 5:
        //            return "Gram Gümüş (CHF)";
        //        default:
        //            return string.Empty;
        //    }
        //}

        public void AccountLogicM(DTOCustomer customer)
        {
            EAccountInfo eAccount1 = new EAccountInfo();
            DTOAccountInfo dTOAccount = new DTOAccountInfo()
            {
                CustomerId = customer.CustomerId,
            };
            List<DTOAccountInfo> accountInfos = eAccount1.ReadListAccountInfo(dTOAccount);

            foreach (DTOAccountInfo accountInfo in accountInfos)
            {
                //string dovizCinsi = GetCurrencyType(accountInfo.DovizCins);

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

            BTransaction transactionLogic = new BTransaction();

            Console.WriteLine("\nWhich account would you like to delete?");
            transactionLogic.PrintAccountList(accountInfos);

            Console.Write("Account Indexi: ");
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
        /// Para Yatırma
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
                    AccountNumber = accountInfo.AccountNo,
                    TargetAccountNumber = accountInfo.AccountNo,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Success,
                    Amount = newBalance - oldBalance,
                    OldBalance = oldBalance,
                    NewBalance = newBalance,
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
                    AccountNumber = accountInfo.AccountNo,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Failed,
                    Amount = balance,
                    OldBalance = accountInfo.Balance,
                    NewBalance = accountInfo.Balance,
                    Timestamp = DateTime.Now
                };

                dataAccessLayer.LogTransaction(transactionLog);

                Console.WriteLine("\nDeposit unsuccessful.");
            }
        }

        /// <summary>
        /// Para Çekme
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <param name="balance"></param>
        public void Withdraw(DTOAccountInfo accountInfo, decimal balance)
        {
            accountInfo = eAccount.ReadAccountInfo(accountInfo);
            if (accountInfo != null)
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
                    AccountNumber = accountInfo.AccountNo,
                    TargetAccountNumber = accountInfo.AccountNo,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Success,
                    Amount = oldBalance - newBalance,
                    OldBalance = oldBalance,
                    NewBalance = newBalance,
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
                    AccountNumber = accountInfo.AccountNo,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Failed,
                    Amount = balance,
                    OldBalance = accountInfo.Balance,
                    NewBalance = accountInfo.Balance,
                    Timestamp = DateTime.Now
                };

                dataAccessLayer.LogTransaction(transactionLog);

                Console.WriteLine("\nWithdraw unsuccessful.");
            }
        }

        /// <summary>
        /// BOA Transfer
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
                        Amount = movement.Miktar,
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
        /// Havale/EFT
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