using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;
using FabrieBank.DAL;
using FabrieBank.DAL.Entity;
using Npgsql;
using System.Reflection;

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

        public string GetDovizCinsi(int dovizCins)
        {
            switch (dovizCins)
            {
                case 1:
                    return "Türk Lirası (TRY)";
                case 2:
                    return "Amerikan Doları (USD)";
                case 3:
                    return "Euro (EUR)";
                case 4:
                    return "Gram Altın (GBP)";
                case 5:
                    return "Gram Gümüş (CHF)";
                default:
                    return string.Empty;
            }
        }

        public void AccountLogicM(DTOCustomer customer)
        {
            EAccountInfo eAccount1 = new EAccountInfo();
            DTOAccountInfo dTOAccount = new DTOAccountInfo()
            {
                MusteriId = customer.MusteriId,
            };
            List<DTOAccountInfo> accountInfos = eAccount1.ReadListAccountInfo(dTOAccount);

            foreach (DTOAccountInfo accountInfo in accountInfos)
            {
                string dovizCinsi = GetDovizCinsi(accountInfo.DovizCins);

                Console.WriteLine($"Hesap No: {accountInfo.HesapNo}");
                Console.WriteLine($"Bakiye: {accountInfo.Bakiye}");
                Console.WriteLine($"DovizCins: {accountInfo.DovizCins}");
                Console.WriteLine($"HesapAdi: {accountInfo.HesapAdi}");
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
        public void HesapSil()
        {
            DTOAccountInfo dTOAccount = new DTOAccountInfo();
            Console.WriteLine("\nSilmek istediğiniz hesap numarasını girin: ");
            Console.Write(">>> ");
            dTOAccount.HesapNo = long.Parse(Console.ReadLine());
            _ = eAccount.DeleteAccountInfo(dTOAccount);
        }

        /// <summary>
        /// Para Yatırma
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <param name="bakiye"></param>
        public void Deposit(DTOAccountInfo accountInfo, decimal bakiye)
        {
            accountInfo = eAccount.ReadAccountInfo(accountInfo);
            if (accountInfo != null)
            {
                decimal eskiBakiye = Convert.ToDecimal(accountInfo.Bakiye);
                decimal yeniBakiye = eskiBakiye + bakiye;

                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                {
                    HesapNo = accountInfo.HesapNo,
                    Bakiye = yeniBakiye,
                    HesapAdi = accountInfo.HesapAdi
                };

                eAccount.UpdateAccountInfo(dTOAccount);

                // Log the successful deposit
                DTOTransactionLog transactionLog = new DTOTransactionLog
                {
                    AccountNumber = accountInfo.HesapNo,
                    TargetAccountNumber = accountInfo.HesapNo,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Success,
                    Amount = yeniBakiye - eskiBakiye,
                    OldBalance = eskiBakiye,
                    NewBalance = yeniBakiye,
                    Timestamp = DateTime.Now
                };

                dataAccessLayer.LogTransaction(transactionLog);

                Console.WriteLine("\nPara yatırma işlemi başarılı.");
                Console.WriteLine($"Eski bakiye: {eskiBakiye}");
                Console.WriteLine($"Yeni bakiye: {yeniBakiye}");
            }
            else
            {
                // Log the failed deposit
                DTOTransactionLog transactionLog = new DTOTransactionLog
                {
                    AccountNumber = accountInfo.HesapNo,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Failed,
                    Amount = bakiye,
                    OldBalance = accountInfo.Bakiye,
                    NewBalance = accountInfo.Bakiye,
                    Timestamp = DateTime.Now
                };

                dataAccessLayer.LogTransaction(transactionLog);

                Console.WriteLine("\nPara yatırma işlemi başarısız.");
            }
        }

        /// <summary>
        /// Para Çekme
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <param name="bakiye"></param>
        public void Withdraw(DTOAccountInfo accountInfo, decimal bakiye)
        {
            accountInfo = eAccount.ReadAccountInfo(accountInfo);
            if (accountInfo != null)
            {
                decimal eskiBakiye = Convert.ToDecimal(accountInfo.Bakiye);
                decimal yeniBakiye = eskiBakiye - bakiye;

                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                {
                    HesapNo = accountInfo.HesapNo,
                    Bakiye = yeniBakiye,
                    HesapAdi = accountInfo.HesapAdi
                };

                eAccount.UpdateAccountInfo(dTOAccount);

                // Log the successful deposit
                DTOTransactionLog transactionLog = new DTOTransactionLog
                {
                    AccountNumber = accountInfo.HesapNo,
                    TargetAccountNumber = accountInfo.HesapNo,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Success,
                    Amount = eskiBakiye - yeniBakiye,
                    OldBalance = eskiBakiye,
                    NewBalance = yeniBakiye,
                    Timestamp = DateTime.Now
                };

                dataAccessLayer.LogTransaction(transactionLog);

                Console.WriteLine("\nPara yatırma işlemi başarılı.");
                Console.WriteLine($"Eski bakiye: {eskiBakiye}");
                Console.WriteLine($"Yeni bakiye: {yeniBakiye}");
            }
            else
            {
                // Log the failed deposit
                DTOTransactionLog transactionLog = new DTOTransactionLog
                {
                    AccountNumber = accountInfo.HesapNo,
                    TransactionType = EnumTransactionType.Deposit,
                    TransactionStatus = EnumTransactionStatus.Failed,
                    Amount = bakiye,
                    OldBalance = accountInfo.Bakiye,
                    NewBalance = accountInfo.Bakiye,
                    Timestamp = DateTime.Now
                };

                dataAccessLayer.LogTransaction(transactionLog);

                Console.WriteLine("\nPara yatırma işlemi başarısız.");
            }
        }

        /// <summary>
        /// BOA Transfer
        /// </summary>
        /// <param name="hareket"></param>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        public bool HesaplarArasiTransfer(DTODovizHareket hareket, DTOAccountInfo accountInfo)
        {
            accountInfo = eAccount.ReadAccountInfo(accountInfo);
            if (accountInfo != null)
            {
                decimal eskiBakiye = accountInfo.Bakiye;
                decimal yeniBakiye = eskiBakiye - hareket.Miktar;

                if (eskiBakiye < hareket.Miktar)
                {
                    // Log the failed transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        AccountNumber = hareket.KaynakHesapNo,
                        TargetAccountNumber = hareket.HedefHesapNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        Amount = hareket.Miktar,
                        OldBalance = eskiBakiye,
                        NewBalance = eskiBakiye,
                        Timestamp = DateTime.Now
                    };
                    dataAccessLayer.LogTransaction(transactionLog);
                    Console.WriteLine("\nYetersiz bakiye. Transfer gerçekleştirilemedi.");
                    return false;
                }
                else
                {
                    DTOAccountInfo updateKaynak = new DTOAccountInfo()
                    {
                        HesapNo = hareket.KaynakHesapNo,
                        Bakiye = yeniBakiye,
                        HesapAdi = accountInfo.HesapAdi
                    };
                    eAccount.UpdateAccountInfo(updateKaynak);
                }

                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                {
                    HesapNo = hareket.HedefHesapNo
                };

                accountInfo = eAccount.ReadAccountInfo(dTOAccount);
                if (dTOAccount != null)
                {
                    DTOAccountInfo updateHedef = new DTOAccountInfo()
                    {
                        HesapNo = hareket.HedefHesapNo,
                        Bakiye = accountInfo.Bakiye + hareket.Miktar,
                        HesapAdi = accountInfo.HesapAdi
                    };

                    eAccount.UpdateAccountInfo(updateHedef);

                    // Log the successful transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        AccountNumber = hareket.KaynakHesapNo,
                        TargetAccountNumber = hareket.HedefHesapNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Success,
                        Amount = hareket.Miktar,
                        OldBalance = eskiBakiye,
                        NewBalance = yeniBakiye,
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
                        AccountNumber = hareket.KaynakHesapNo,
                        TargetAccountNumber = hareket.HedefHesapNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        Amount = hareket.Miktar,
                        OldBalance = eskiBakiye,
                        NewBalance = eskiBakiye,
                        Timestamp = DateTime.Now
                    };

                    DTOAccountInfo updateKaynak = new DTOAccountInfo()
                    {
                        HesapNo = hareket.KaynakHesapNo,
                        Bakiye = eskiBakiye,
                        HesapAdi = accountInfo.HesapAdi
                    };

                    dataAccessLayer.LogTransaction(transactionLog);
                    eAccount.UpdateAccountInfo(updateKaynak);

                    Console.WriteLine("\nHedef hesap bulunamadı.");
                    return false;
                }
            }
            else
            {
                // Log the failed transfer
                DTOTransactionLog transactionLog = new DTOTransactionLog
                {
                    AccountNumber = hareket.KaynakHesapNo,
                    TargetAccountNumber = hareket.HedefHesapNo,
                    TransactionType = EnumTransactionType.BOATransfer,
                    TransactionStatus = EnumTransactionStatus.Failed,
                    Amount = hareket.Miktar,
                    OldBalance = accountInfo.Bakiye,
                    NewBalance = accountInfo.Bakiye,
                    Timestamp = DateTime.Now
                };
                dataAccessLayer.LogTransaction(transactionLog);
                Console.WriteLine("\nKaynak hesap bulunamadı.");
                return false;
            }
        }

        /// <summary>
        /// Havale İşlemi
        /// </summary>
        /// <param name="kaynakHesapNo"></param>
        /// <param name="hedefHesapNo"></param>
        /// <param name="miktar"></param>
        /// <returns></returns>
        public bool Havale(DTODovizHareket hareket, DTOAccountInfo accountInfo)
        {
            accountInfo = eAccount.ReadAccountInfo(accountInfo);
            if (accountInfo != null)
            {
                decimal transactionFee = dataAccessLayer.GetTransactionFee(EnumTransactionFeeType.Havale);
                decimal eskiBakiye = accountInfo.Bakiye;
                decimal yeniBakiye = eskiBakiye - hareket.Miktar - transactionFee;

                if (eskiBakiye < hareket.Miktar)
                {
                    Console.WriteLine("\nYetersiz bakiye. Transfer gerçekleştirilemedi.");

                    // Log the failed transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        AccountNumber = hareket.KaynakHesapNo,
                        TargetAccountNumber = hareket.HedefHesapNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        Amount = hareket.Miktar,
                        OldBalance = eskiBakiye,
                        NewBalance = eskiBakiye,
                        Timestamp = DateTime.Now
                    };
                    dataAccessLayer.LogTransaction(transactionLog);
                }
                else
                {
                    DTOAccountInfo updateKaynak = new DTOAccountInfo()
                    {
                        HesapNo = hareket.KaynakHesapNo,
                        Bakiye = yeniBakiye,
                        HesapAdi = accountInfo.HesapAdi
                    };
                    eAccount.UpdateAccountInfo(updateKaynak);
                }

                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                {
                    HesapNo = hareket.HedefHesapNo
                };

                accountInfo = eAccount.ReadAccountInfo(dTOAccount);
                if (dTOAccount != null)
                {
                    DTOAccountInfo updateHedef = new DTOAccountInfo()
                    {
                        HesapNo = hareket.HedefHesapNo,
                        Bakiye = accountInfo.Bakiye + hareket.Miktar,
                        HesapAdi = accountInfo.HesapAdi
                    };

                    eAccount.UpdateAccountInfo(updateHedef);

                    // Log the successful transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        AccountNumber = hareket.KaynakHesapNo,
                        TargetAccountNumber = hareket.HedefHesapNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Success,
                        Amount = hareket.Miktar,
                        OldBalance = eskiBakiye,
                        NewBalance = yeniBakiye,
                        Timestamp = DateTime.Now
                    };

                    dataAccessLayer.LogTransaction(transactionLog);

                    return true;
                }
                else
                {
                    Console.WriteLine("Hedef hesap numarası bankamıza ait değil lütfen EFT işlemi gerçekleştiriniz.");

                    // Log the failed transfer
                    DTOTransactionLog transactionLog = new DTOTransactionLog
                    {
                        AccountNumber = hareket.KaynakHesapNo,
                        TargetAccountNumber = hareket.HedefHesapNo,
                        TransactionType = EnumTransactionType.BOATransfer,
                        TransactionStatus = EnumTransactionStatus.Failed,
                        Amount = hareket.Miktar,
                        OldBalance = eskiBakiye,
                        NewBalance = eskiBakiye,
                        Timestamp = DateTime.Now
                    };

                    DTOAccountInfo updateKaynak = new DTOAccountInfo()
                    {
                        HesapNo = hareket.KaynakHesapNo,
                        Bakiye = eskiBakiye,
                        HesapAdi = accountInfo.HesapAdi
                    };

                    dataAccessLayer.LogTransaction(transactionLog);
                    eAccount.UpdateAccountInfo(updateKaynak);

                    return false;
                }
            }
            else
            {
                Console.WriteLine("Kaynak hesap numarası bulunamadı.");
            }
            return true;
        }
    }
}

