﻿using FabrieBank.Admin.DAL.DTO;
using FabrieBank.Admin.DAL.Entity;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;
using FabrieBank.DAL.Entity;

namespace FabrieBank.Admin.BLL.Logic
{
    public class BAdmin
    {
        private EAdmin eAdmin;
        private ECustomer eCustomer;
        private EAccountInfo eAccount;
        private EErrorLog eError;
        private ETransactionLog eTransaction;
        private ETransactionFee eTransactionFee;

        public BAdmin()
        {
            eAdmin = new EAdmin();
            eCustomer = new ECustomer();
            eAccount = new EAccountInfo();
            eError = new EErrorLog();
            eTransaction = new ETransactionLog();
            eTransactionFee = new ETransactionFee();
        }

        /// <summary>
        /// Admin login logic
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public DTOAdmin LogIn(string nickname, string password)
        {
            DTOAdmin admin = eAdmin.ReadAdmin(new DTOAdmin { Nickname = nickname });

            if (admin != null && admin.Password == password)
            {
                return admin;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Customer status update logic
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool CustomerStatusUpdate(DTOCustomer customer)
        {
            eCustomer.UpdateCustomer(customer);
            return true;
        }

        /// <summary>
        /// ReadList and ReadById customer logic
        /// </summary>
        /// <param name="customer"></param>
        public void ListCustomers(DTOCustomer customer)
        {
            Console.WriteLine("\n");
            if (customer.Tckn != 0)
            {
                DTOCustomer dTOCustomer = eCustomer.ReadCustomer(customer);

                    Console.WriteLine($"ID      : {dTOCustomer.CustomerId}");
                    Console.WriteLine($"Name    : {dTOCustomer.Name}");
                    Console.WriteLine($"Lastname: {dTOCustomer.Lastname}");
                    Console.WriteLine($"TCKN    : {dTOCustomer.Tckn}");
                    Console.WriteLine($"Password: {dTOCustomer.Password}");
                    Console.WriteLine($"Cell No : {dTOCustomer.CellNo}");
                    Console.WriteLine($"Email   : {dTOCustomer.Email}");
                    Console.WriteLine($"Status  : {dTOCustomer.Status}");
                    Console.WriteLine("================================\n");

                if (dTOCustomer == null)
                {
                    Console.WriteLine("No customer was found that met the given parameters.");
                }
            }
            else
            {
                List<DTOCustomer> dTOCustomers = eCustomer.ReadListCustomer(customer);

                foreach (DTOCustomer dTOCustomer in dTOCustomers)
                {
                    Console.WriteLine($"ID      : {dTOCustomer.CustomerId}");
                    Console.WriteLine($"Name    : {dTOCustomer.Name}");
                    Console.WriteLine($"Lastname: {dTOCustomer.Lastname}");
                    Console.WriteLine($"TCKN    : {dTOCustomer.Tckn}");
                    Console.WriteLine($"Password: {dTOCustomer.Password}");
                    Console.WriteLine($"Cell No : {dTOCustomer.CellNo}");
                    Console.WriteLine($"Email   : {dTOCustomer.Email}");
                    Console.WriteLine($"Status  : {dTOCustomer.Status}");
                    Console.WriteLine("================================\n");
                }

                if (dTOCustomers.Count == 0)
                {
                    Console.WriteLine("No customer was found that met the given parameters.");
                }
            }
        }

        /// <summary>
        /// ReadList and ReadById account info logic
        /// </summary>
        /// <param name="accountInfo"></param>
        public void ListAccounts(DTOAccountInfo accountInfo)
        {
            Console.WriteLine("\n");
            if (accountInfo.AccountNo != 0)
            {
                DTOAccountInfo dTOAccountInfo = eAccount.ReadAccountInfo(accountInfo);

                Console.WriteLine($"Account No       : {dTOAccountInfo.AccountNo}");
                Console.WriteLine($"Balance          : {dTOAccountInfo.Balance}");
                Console.WriteLine($"Customer ID      : {dTOAccountInfo.CustomerId}");
                Console.WriteLine($"Currency Type    : {dTOAccountInfo.CurrencyType}");
                Console.WriteLine($"Account Name     : {dTOAccountInfo.AccountName}");
                Console.WriteLine("========================================\n");

                if (dTOAccountInfo == null)
                {
                    Console.WriteLine("No account was found that met the given parameters.");
                }
            }
            else
            {
                List<DTOAccountInfo> dTOAccountInfos = eAccount.ReadListAccountInfo(accountInfo);

                foreach (DTOAccountInfo dTOAccount in dTOAccountInfos)
                {
                    Console.WriteLine($"Account No       : {dTOAccount.AccountNo}");
                    Console.WriteLine($"Balance          : {dTOAccount.Balance}");
                    Console.WriteLine($"Customer ID      : {dTOAccount.CustomerId}");
                    Console.WriteLine($"Currency Type    : {dTOAccount.CurrencyType}");
                    Console.WriteLine($"Account Name     : {dTOAccount.AccountName}");
                    Console.WriteLine("========================================\n");
                }

                if (dTOAccountInfos.Count == 0)
                {
                    Console.WriteLine("No account was found that met the given parameters.");
                }
            }
        }

        /// <summary>
        /// ReadList and ReadById error log logic
        /// </summary>
        /// <param name="errorLog"></param>
        public void ListErrorLogs(DTOErrorLog errorLog)
        {
            Console.WriteLine("\n");
            if (errorLog.ErrorId != 0)
            {
                DTOErrorLog dTOErrorLog = eError.ReadErrorLog(errorLog);

                Console.WriteLine($"Error ID       : {dTOErrorLog.ErrorId}");
                Console.WriteLine($"Error Date Time: {dTOErrorLog.ErrorDateTime}");
                Console.WriteLine($"Error Message  : {dTOErrorLog.ErrorMessage}");
                Console.WriteLine($"Stack Trace    : {dTOErrorLog.StackTrace}");
                Console.WriteLine($"Operation Name : {dTOErrorLog.OperationName}");
                Console.WriteLine("========================================\n");

                if (dTOErrorLog == null)
                {
                    Console.WriteLine("No log was found that met the given parameters.");
                }
            }
            else
            {
                List<DTOErrorLog> dTOErrorLogs = eError.ReadListErrorLog(errorLog);

                foreach (DTOErrorLog dTOError in dTOErrorLogs)
                {
                    Console.WriteLine($"Error ID       : {dTOError.ErrorId}");
                    Console.WriteLine($"Error Date Time: {dTOError.ErrorDateTime}");
                    Console.WriteLine($"Error Message  : {dTOError.ErrorMessage}");
                    Console.WriteLine($"Stack Trace    : {dTOError.StackTrace}");
                    Console.WriteLine($"Operation Name : {dTOError.OperationName}");
                    Console.WriteLine("========================================\n");
                }

                if (dTOErrorLogs.Count == 0)
                {
                    Console.WriteLine("No log was found that met the given parameters.");
                }
            }
        }

        /// <summary>
        /// ReadList and ReadById transaction log logic
        /// </summary>
        /// <param name="transactionLog"></param>
        public void ListTransactionLogs(DTOTransactionLog transactionLog)
        {
            Console.WriteLine("\n");
            if (transactionLog.LogId != 0)
            {
                DTOTransactionLog dTOTransactionLog = eTransaction.ReadTransactionLog(transactionLog);

                Console.WriteLine($"Log ID               : {dTOTransactionLog.LogId}");
                Console.WriteLine($"Source Account Number: {dTOTransactionLog.SourceAccountNumber}");
                Console.WriteLine($"Target Account Number: {dTOTransactionLog.TargetAccountNumber}");
                Console.WriteLine($"Source Old Balance   : {dTOTransactionLog.SourceOldBalance}");
                Console.WriteLine($"Target Old Balance   : {dTOTransactionLog.TargetOldBalance}");
                Console.WriteLine($"Source New Balance   : {dTOTransactionLog.SourceNewBalance}");
                Console.WriteLine($"Target New Balance   : {dTOTransactionLog.TargetNewBalance}");
                Console.WriteLine($"Transfer Amount      : {dTOTransactionLog.TransferAmount}");
                Console.WriteLine($"Transaction Type     : {dTOTransactionLog.TransactionType}");
                Console.WriteLine($"Transaction Status   : {dTOTransactionLog.TransactionStatus}");
                Console.WriteLine($"Source Currency Type : {dTOTransactionLog.SourceCurrencyType}");
                Console.WriteLine($"Target Currency Type : {dTOTransactionLog.TargetCurrencyType}");
                Console.WriteLine($"Currency Rate        : {dTOTransactionLog.CurrencyRate}");
                Console.WriteLine($"KMV                  : {dTOTransactionLog.KMV}");
                Console.WriteLine($"Transaction Fee      : {dTOTransactionLog.TransactionFee}");
                Console.WriteLine($"Timestamp            : {dTOTransactionLog.Timestamp}");
                Console.WriteLine("========================================\n");

                if (dTOTransactionLog == null)
                {
                    Console.WriteLine("No log was found that met the given parameters.");
                }
            }
            else
            {
                List<DTOTransactionLog> dTOTransactionLogs = eTransaction.ReadListTransactionLog(transactionLog);

                foreach (DTOTransactionLog dTOTransaction in dTOTransactionLogs)
                {
                    Console.WriteLine($"Log ID               : {dTOTransaction.LogId}");
                    Console.WriteLine($"Source Account Number: {dTOTransaction.SourceAccountNumber}");
                    Console.WriteLine($"Target Account Number: {dTOTransaction.TargetAccountNumber}");
                    Console.WriteLine($"Source Old Balance   : {dTOTransaction.SourceOldBalance}");
                    Console.WriteLine($"Source New Balance   : {dTOTransaction.SourceNewBalance}");
                    Console.WriteLine($"Target Old Balance   : {dTOTransaction.TargetOldBalance}");
                    Console.WriteLine($"Target New Balance   : {dTOTransaction.TargetNewBalance}");
                    Console.WriteLine($"Transaction Type     : {dTOTransaction.TransactionType}");
                    Console.WriteLine($"Transaction Status   : {dTOTransaction.TransactionStatus}");
                    Console.WriteLine($"Transfer Amount      : {dTOTransaction.TransferAmount}");
                    Console.WriteLine($"Source Currency Type : {dTOTransaction.SourceCurrencyType}");
                    Console.WriteLine($"Target Currency Type : {dTOTransaction.TargetCurrencyType}");
                    Console.WriteLine($"Currency Rate        : {dTOTransaction.CurrencyRate}");
                    Console.WriteLine($"KMV                  : {dTOTransaction.KMV}");
                    Console.WriteLine($"Transaction Fee      : {dTOTransaction.TransactionFee}");
                    Console.WriteLine($"Timestamp            : {dTOTransaction.Timestamp}");
                    Console.WriteLine("========================================\n");
                }

                if (dTOTransactionLogs.Count == 0)
                {
                    Console.WriteLine("No log was found that met the given parameters.");
                }
            }
        }

        /// <summary>
        /// ReadList transaction fee logic
        /// </summary>
        /// <param name="dTOTransactionFee"></param>
        public void ListFees(DTOTransactionFee dTOTransactionFee)
        {
            Console.WriteLine("\n");
            List<DTOTransactionFee> dTOTransactionFees = eTransactionFee.ReadListTransactionFee(dTOTransactionFee);

            foreach (DTOTransactionFee transactionFee in dTOTransactionFees)
            {
                Console.WriteLine($"Fee Type  : {transactionFee.FeeType}");
                Console.WriteLine($"Fee Amount: {transactionFee.Amount}");
                Console.WriteLine("========================================\n");
            }

            if (dTOTransactionFees.Count == 0)
            {
                Console.WriteLine("No record was found that met the given parameters.");
            }
        }

        /// <summary>
        /// Add new fee logic
        /// </summary>
        /// <param name="dTOTransactionFee"></param>
        /// <returns></returns>
        public bool AddFee(DTOTransactionFee dTOTransactionFee)
        {
            eTransactionFee.InsertTransactionFee(dTOTransactionFee);
            return true;
        }

        /// <summary>
        /// Remove fee logic
        /// </summary>
        /// <param name="dTOTransactionFee"></param>
        /// <returns></returns>
        public bool RemoveFee(DTOTransactionFee dTOTransactionFee)
        {
            eTransactionFee.DeleteTransactionFee(dTOTransactionFee);
            return true;
        }

        /// <summary>
        /// Update fee logic
        /// </summary>
        /// <param name="dTOTransactionFee"></param>
        /// <returns></returns>
        public bool UpdateFee(DTOTransactionFee dTOTransactionFee)
        {
            eTransactionFee.UpdateTransactionFee(dTOTransactionFee);
            return true;
        }

        /// <summary>
        /// Monthly KMV calculator
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="monthlyTransactions"></param>
        public void MonthlyKMV(DTOAdmin admin ,DTOMonthlyTransactions monthlyTransactions)
        {
            if (admin.AccessLevel == 3)
            {
                DTOTransactionLog dTOTransactionLog = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth
                };

                List<DTOTransactionLog> dTOTransactionLogs = eTransaction.ReadListTransactionLog(dTOTransactionLog);

                decimal totalKMV = 0;

                foreach (DTOTransactionLog transaction in dTOTransactionLogs)
                {
                    totalKMV += transaction.KMV;
                }

                Console.WriteLine($"\nTotal KMV for this month: {totalKMV}\n");
            }
            else
            {
                Console.WriteLine("Permission denied. Your access level is insufficient for this process.");
            }
        }

        /// <summary>
        /// Monthly transaction volume calculator
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="monthlyTransactions"></param>
        public void MonthlyTransactionVolume(DTOAdmin admin, DTOMonthlyTransactions monthlyTransactions)
        {
            if (admin.AccessLevel == 3)
            {
                DTOTransactionLog dTOTransactionLogDepositTRY = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Success,
                    TargetCurrencyType = EnumCurrencyTypes.CurrencyTypes.TRY
                };

                List<DTOTransactionLog> dTOTransactionLogs = eTransaction.ReadListTransactionLog(dTOTransactionLogDepositTRY);

                decimal totalDepositTRY = 0;

                foreach (DTOTransactionLog transaction in dTOTransactionLogs)
                {
                    totalDepositTRY += transaction.TransferAmount;
                }

                Console.WriteLine($"\nTotal TRY Deposit for this month: {totalDepositTRY}\n");

                DTOTransactionLog dTOTransactionLogDepositUSD = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Success,
                    TargetCurrencyType = EnumCurrencyTypes.CurrencyTypes.USD
                };

                List<DTOTransactionLog> dTOTransactionLogsUSD = eTransaction.ReadListTransactionLog(dTOTransactionLogDepositUSD);

                decimal totalDepositUSD = 0;

                foreach (DTOTransactionLog transactionUSD in dTOTransactionLogsUSD)
                {
                    totalDepositUSD += transactionUSD.TransferAmount;
                }

                Console.WriteLine($"\nTotal USD Deposit for this month: {totalDepositUSD}\n");

                DTOTransactionLog dTOTransactionLogDepositEUR = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Success,
                    TargetCurrencyType = EnumCurrencyTypes.CurrencyTypes.EUR
                };

                List<DTOTransactionLog> dTOTransactionLogsEUR = eTransaction.ReadListTransactionLog(dTOTransactionLogDepositEUR);

                decimal totalDepositEUR = 0;

                foreach (DTOTransactionLog transactionEUR in dTOTransactionLogsEUR)
                {
                    totalDepositEUR += transactionEUR.TransferAmount;
                }

                Console.WriteLine($"\nTotal EUR Deposit for this month: {totalDepositEUR}\n");

                DTOTransactionLog dTOTransactionLogDepositGBP = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Success,
                    TargetCurrencyType = EnumCurrencyTypes.CurrencyTypes.GBP
                };

                List<DTOTransactionLog> dTOTransactionLogsGBP = eTransaction.ReadListTransactionLog(dTOTransactionLogDepositGBP);

                decimal totalDepositGBP = 0;

                foreach (DTOTransactionLog transactionGBP in dTOTransactionLogsGBP)
                {
                    totalDepositGBP += transactionGBP.TransferAmount;
                }

                Console.WriteLine($"\nTotal GBP Deposit for this month: {totalDepositGBP}\n");

                DTOTransactionLog dTOTransactionLogDepositCHF = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Success,
                    TargetCurrencyType = EnumCurrencyTypes.CurrencyTypes.CHF
                };

                List<DTOTransactionLog> dTOTransactionLogsCHF = eTransaction.ReadListTransactionLog(dTOTransactionLogDepositCHF);

                decimal totalDepositCHF = 0;

                foreach (DTOTransactionLog transactionCHF in dTOTransactionLogsCHF)
                {
                    totalDepositCHF += transactionCHF.TransferAmount;
                }

                Console.WriteLine($"\nTotal CHF Deposit for this month: {totalDepositCHF}\n");

                decimal totalDeposit = totalDepositTRY + totalDepositUSD + totalDepositEUR + totalDepositGBP + totalDepositCHF;

                Console.WriteLine($"\nTotal Deposit this month: {totalDeposit}\n");

                DTOTransactionLog dTOTransactionLogWithdrawTRY = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth,
                    TransactionType = EnumTransactionType.Withdrawal,
                    TransactionStatus = EnumTransactionStatus.Success,
                    TargetCurrencyType = EnumCurrencyTypes.CurrencyTypes.TRY
                };

                List<DTOTransactionLog> dTOTransactionLogsWithdrawTRY = eTransaction.ReadListTransactionLog(dTOTransactionLogWithdrawTRY);

                decimal totalWithdrawTRY = 0;

                foreach (DTOTransactionLog transactionWithdrawTRY in dTOTransactionLogsWithdrawTRY)
                {
                    totalWithdrawTRY += transactionWithdrawTRY.TransferAmount;
                }

                Console.WriteLine($"\nTotal TRY Withdraw for this month: {totalWithdrawTRY}\n");

                DTOTransactionLog dTOTransactionLogWithdrawUSD = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth,
                    TransactionType = EnumTransactionType.Withdrawal,
                    TransactionStatus = EnumTransactionStatus.Success,
                    SourceCurrencyType = EnumCurrencyTypes.CurrencyTypes.USD
                };

                List<DTOTransactionLog> dTOTransactionLogsUSDWithdraw = eTransaction.ReadListTransactionLog(dTOTransactionLogWithdrawUSD);

                decimal totalWithdrawUSD = 0;

                foreach (DTOTransactionLog transactionUSDWithdraw in dTOTransactionLogsUSDWithdraw)
                {
                    totalWithdrawUSD += transactionUSDWithdraw.TransferAmount;
                }

                Console.WriteLine($"\nTotal USD Withdraw for this month: {totalWithdrawUSD}\n");

                DTOTransactionLog dTOTransactionLogWithdrawEUR = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth,
                    TransactionType = EnumTransactionType.Withdrawal,
                    TransactionStatus = EnumTransactionStatus.Success,
                    SourceCurrencyType = EnumCurrencyTypes.CurrencyTypes.EUR
                };

                List<DTOTransactionLog> dTOTransactionLogsEURWithdraw = eTransaction.ReadListTransactionLog(dTOTransactionLogWithdrawEUR);

                decimal totalWithdrawEUR = 0;

                foreach (DTOTransactionLog transactionEURWithdraw in dTOTransactionLogsEURWithdraw)
                {
                    totalWithdrawEUR += transactionEURWithdraw.TransferAmount;
                }

                Console.WriteLine($"\nTotal EUR Withdraw for this month: {totalWithdrawEUR}\n");

                DTOTransactionLog dTOTransactionLogWithdrawGBP = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth,
                    TransactionType = EnumTransactionType.Withdrawal,
                    TransactionStatus = EnumTransactionStatus.Success,
                    SourceCurrencyType = EnumCurrencyTypes.CurrencyTypes.GBP
                };

                List<DTOTransactionLog> dTOTransactionLogsGBPWithdraw = eTransaction.ReadListTransactionLog(dTOTransactionLogWithdrawGBP);

                decimal totalWithdrawGBP = 0;

                foreach (DTOTransactionLog transactionGBPWithdraw in dTOTransactionLogsGBPWithdraw)
                {
                    totalWithdrawGBP += transactionGBPWithdraw.TransferAmount;
                }

                Console.WriteLine($"\nTotal GBP Withdraw for this month: {totalWithdrawGBP}\n");

                DTOTransactionLog dTOTransactionLogWithdrawCHF = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth,
                    TransactionType = EnumTransactionType.Withdrawal,
                    TransactionStatus = EnumTransactionStatus.Success,
                    SourceCurrencyType = EnumCurrencyTypes.CurrencyTypes.CHF
                };

                List<DTOTransactionLog> dTOTransactionLogsCHFWithdraw = eTransaction.ReadListTransactionLog(dTOTransactionLogWithdrawCHF);

                decimal totalWithdrawCHF = 0;

                foreach (DTOTransactionLog transactionCHFWithdraw in dTOTransactionLogsCHFWithdraw)
                {
                    totalWithdrawCHF += transactionCHFWithdraw.TransferAmount;
                }

                Console.WriteLine($"\nTotal CHF Withdraw for this month: {totalWithdrawCHF}\n");

                decimal totalWithdraw = totalWithdrawTRY + totalWithdrawUSD + totalWithdrawEUR + totalWithdrawGBP + totalWithdrawCHF;

                Console.WriteLine($"\nTotal Withdraw this month: {totalWithdraw}\n");

                decimal total = totalDeposit - totalWithdraw;

                Console.WriteLine($"\nTotal: {total}\n");
            }
            else
            {
                Console.WriteLine("Permission denied. Your access level is insufficient for this process.");
            }
        }

        /// <summary>
        /// Monthly total fee calculator
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="monthlyTransactions"></param>
        public void MonthlyFeeTotal(DTOAdmin admin, DTOMonthlyTransactions monthlyTransactions)
        {
            if (admin.AccessLevel == 3)
            {
                DTOTransactionLog dTOTransactionLog = new DTOTransactionLog()
                {
                    StartDate = monthlyTransactions.FirstDayOfMonth,
                    EndDate = monthlyTransactions.ThisDayOfMonth
                };

                List<DTOTransactionLog> dTOTransactionLogs = eTransaction.ReadListTransactionLog(dTOTransactionLog);

                decimal totalFee = 0;

                foreach (DTOTransactionLog transaction in dTOTransactionLogs)
                {
                    totalFee += transaction.TransactionFee;
                }

                Console.WriteLine($"\nTotal Fee for this month: {totalFee}\n");
            }
            else
            {
                Console.WriteLine("Permission denied. Your access level is insufficient for this process.");
            }
        }
    }
}