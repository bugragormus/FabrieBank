﻿using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;
using FabrieBank.BLL.Logic;
using FabrieBank.DAL.Common.Enums;
using FabrieBank.Services;
using System.Security.Principal;

namespace FabrieBank
{
    public class PTransaction
    {
        private int customerId;
        private BTransaction transactionLogic;
        private BAccount bAccount;
        private readonly SCurrency currency;
        private EnumCurrencyTypes.CurrencyTypes baseCurrency;
        private EAccountInfo eAccount;
        private PCurrency pCurrency;

        public PTransaction(int customerId)
        {
            this.customerId = customerId;
            transactionLogic = new BTransaction();
            bAccount = new BAccount();
            currency = new SCurrency();
            baseCurrency = EnumCurrencyTypes.CurrencyTypes.TRY;
            eAccount = new EAccountInfo();
            pCurrency = new PCurrency();
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

                        transactionLogic.Deposit(accountInfo, amount);
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

                        transactionLogic.Withdraw(accountInfo, amount);
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

        public void HavaleEFT(List<DTOAccountInfo> accountInfos)
        {
            Console.WriteLine("\nWhich account do you want to withdraw money from?");
            bAccount.PrintAccountList(accountInfos);

            Console.Write("Source Account Index: ");
            int sourceAccountIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nEnter the account number you want to transfer money to: ");
            Console.Write("Target Account Number: ");
            long targetAccountNo = long.Parse(Console.ReadLine());

            Console.WriteLine("\nEnter the amount you want to transfer: ");
            decimal transferAmount = decimal.Parse(Console.ReadLine());

            DTOTransfer transfer = new DTOTransfer()
            {
                SourceAccountIndex = sourceAccountIndex,
                TargetAccountNo = targetAccountNo,
                Amount = transferAmount
            };

            transactionLogic.HavaleEFT(customerId, transfer);
        }

        public void TransferBetweenAccounts(List<DTOAccountInfo> accountInfos)
        {
            Console.WriteLine("\nWhich account do you want to withdraw money from?");
            bAccount.PrintAccountList(accountInfos);

            Console.Write("Source Account Index: ");
            int sourceAccountIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nWhich account do you want to transfer money to?");
            bAccount.PrintAccountList(accountInfos);

            Console.Write("Target Account Index: ");
            int targetAccountIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nEnter the amount you want to transfer: ");
            decimal transferAmount = decimal.Parse(Console.ReadLine());

            DTOTransfer transfer = new DTOTransfer()
            {
                SourceAccountIndex = sourceAccountIndex,
                TargetAccountIndex = targetAccountIndex,
                Amount = transferAmount
            };

            transactionLogic.TransferBetweenAccounts(customerId, transfer);
        }

        public void ExchangeBuying(DTOCustomer customer)
        {
            try
            {
                var currencyRates = currency.GetTodaysCurrencyRates(baseCurrency).Result;
                List<DTOCurrencyRate> dTOCurrencyRates = pCurrency.GetCurrencyRates(currencyRates);

                if (dTOCurrencyRates.Count > 0)
                {
                    Console.WriteLine("\nForex Selling Rate for;  ");
                    for (int i = 0; i < dTOCurrencyRates.Count; i++)
                    {
                        decimal forexSellingRate = dTOCurrencyRates[i].ForexSellingRate;
                        EnumCurrencyTypes.CurrencyTypes currencyType = dTOCurrencyRates[i].CurrencyCode;
                        Console.WriteLine($"\n[{i}] {currencyType}: {forexSellingRate}");
                    }
                    Console.WriteLine("What type of currency would you like to trade?");
                    Console.Write(">>> ");
                    int exchange = int.Parse(Console.ReadLine());
                    decimal exchangeRate = dTOCurrencyRates[exchange].ForexSellingRate;
                    EnumCurrencyTypes.CurrencyTypes exchangeType = dTOCurrencyRates[exchange].CurrencyCode;

                    DTOAccountInfo dTOAccounts = new DTOAccountInfo()
                    {
                        CustomerId = customer.CustomerId,
                        CurrencyType = (int)baseCurrency
                    };

                    List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccounts);

                    if (accountInfos.Count != 0)
                    {
                        Console.WriteLine("\nFrom which account would you like to withdraw the money?\n");
                        bAccount.PrintAccountList(accountInfos);

                        Console.Write("Account Index: ");
                        int withdrawAccIndex = int.Parse(Console.ReadLine());

                        if (withdrawAccIndex >= 0 && withdrawAccIndex < accountInfos.Count)
                        {
                            long withdrawAccNo = accountInfos[withdrawAccIndex].AccountNo;
                            decimal withdrawAccBalance = accountInfos[withdrawAccIndex].Balance;
                            string? sourceAccName = accountInfos[withdrawAccIndex].AccountName;

                            if (withdrawAccBalance != 0)
                            {
                                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                                {
                                    CustomerId = customer.CustomerId,
                                    CurrencyType = (int)exchangeType
                                };

                                List<DTOAccountInfo> accountInfo = eAccount.ReadListAccountInfo(dTOAccount);

                                if (accountInfo.Count != 0)
                                {
                                    Console.WriteLine("\nFrom which account would you like to withdraw the money?\n");
                                    bAccount.PrintAccountList(accountInfo);

                                    Console.Write("Account Index: ");
                                    int depositAccIndex = int.Parse(Console.ReadLine());

                                    if (depositAccIndex >= 0 && depositAccIndex < accountInfo.Count)
                                    {
                                        long depositAccNo = accountInfo[depositAccIndex].AccountNo;
                                        decimal depositAccBalance = accountInfo[depositAccIndex].Balance;
                                        string? targetAccName = accountInfo[depositAccIndex].AccountName;

                                        Console.WriteLine("\nAmount: \n");
                                        decimal amount = int.Parse(Console.ReadLine());

                                        DTOExchange dTOExchange = new DTOExchange()
                                        {
                                            ExchangeRate = exchangeRate,
                                            CurrencyType = exchangeType,
                                            SourceAccountNo = withdrawAccNo,
                                            SourceAccountBalance = withdrawAccBalance,
                                            TargetAccountNo = depositAccNo,
                                            TargetAccountBalance = depositAccBalance,
                                            Amount = amount,
                                            SourceAccountName = sourceAccName,
                                            TargetAccountName = targetAccName
                                        };

                                        transactionLogic.ExchangeBuying(dTOExchange);
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nNo.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("\nTarget account not found.");
                                }

                            }
                            else
                            {
                                Console.WriteLine("\nInsufficient balance.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nNo currency rates available.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nSource account not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

        public void ExchangeSelling(DTOCustomer customer)
        {

        }
    }
}