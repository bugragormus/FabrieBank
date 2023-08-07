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
                            Console.WriteLine("Transfer Between Accounts transaction could not be done. Please try again.");
                        }
                    }
                    else
                    {
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
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
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
                        Console.WriteLine("Invalid account index. Try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
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

        public void PrintAccountList(List<DTOAccountInfo> accountInfos)
        {
            try
            {
                Console.WriteLine("Your Accounts:");
                for (int i = 0; i < accountInfos.Count; i++)
                {
                    Console.WriteLine($"[{i}] Account No: {accountInfos[i].AccountNo}");
                    Console.WriteLine($"Balance: {accountInfos[i].Balance}");
                    Console.WriteLine($"Currency Type: {accountInfos[i].CurrencyType}");
                    Console.WriteLine("==============================");
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