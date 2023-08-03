using System.Reflection;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank.BLL.Logic
{
    public class BTransaction
    {
        private EAccountInfo eAccount;
        private TransferDB transferDB;
        private BAccount account;

        public BTransaction()
        {
            eAccount = new EAccountInfo();
            transferDB = new TransferDB();
            account = new BAccount();
        }

        public void HesaplarArasiTransfer(int musteriId, DTOTransfer transfer)
        {
            try
            {
                DTOAccountInfo dTOAccount = new DTOAccountInfo()
                {
                    MusteriId = musteriId
                };
                List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccount);

                if (transfer.KaynakHesapIndex >= 0 && transfer.KaynakHesapIndex < accountInfos.Count && transfer.HedefHesapIndex >= 0 && transfer.HedefHesapIndex < accountInfos.Count)
                {
                    long kaynakHesapNo = accountInfos[transfer.KaynakHesapIndex].HesapNo;
                    long hedefHesapNo = accountInfos[transfer.HedefHesapIndex].HesapNo;
                    int kaynakDovizCinsi = accountInfos[transfer.KaynakHesapIndex].DovizCins;
                    int hedefDovizCinsi = accountInfos[transfer.HedefHesapIndex].DovizCins;

                    DTODovizHareket dovizHareket = new DTODovizHareket
                    {
                        KaynakHesapNo = kaynakHesapNo,
                        HedefHesapNo = hedefHesapNo,
                        KaynakDovizCinsi = kaynakDovizCinsi,
                        HedefDovizCinsi = hedefDovizCinsi,
                        Miktar = transfer.Miktar
                    };

                    if (dovizHareket.KaynakDovizCinsi == dovizHareket.HedefDovizCinsi)
                    {

                        DTOAccountInfo accountInfo = new DTOAccountInfo()
                        {
                            HesapNo = kaynakHesapNo
                        };

                        bool transferBasarili = account.HesaplarArasiTransfer(dovizHareket, accountInfo);
                        if (transferBasarili)
                        {
                            Console.WriteLine("HesaplarArasiTransfer işlemi başarıyla gerçekleştirildi.");
                        }
                        else
                        {
                            Console.WriteLine("HesaplarArasiTransfer işlemi gerçekleştirilemedi. Lütfen tekrar deneyin.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Kaynak hesap ve hedef hesap döviz cinsleri uyuşmuyor. Transfer işlemi gerçekleştirilemedi.");
                    }
                }
                else
                {
                    Console.WriteLine("Geçersiz hesap indexi. Tekrar deneyin.");
                }
            }
            catch (Exception ex)
            {
                LogAndHandleError(ex);
            }
        }

        public void Havale(int musteriId, DTOTransfer transfer)
        {
            try
            {
                DTOAccountInfo dTOAccount1 = new DTOAccountInfo()
                {
                    HesapNo = transfer.HedefHesapNo
                };

                DTOAccountInfo hedefAccountInfo = eAccount.ReadAccountInfo(dTOAccount1);
                if (hedefAccountInfo.MusteriId != 0)
                {
                    int hedefDovizCinsi = hedefAccountInfo.DovizCins;

                    DTOAccountInfo dTOAccount = new DTOAccountInfo()
                    {
                        MusteriId = musteriId
                    };
                    List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccount);

                    if (transfer.KaynakHesapIndex >= 0 && transfer.KaynakHesapIndex < accountInfos.Count)
                    {
                        long kaynakHesapNo = accountInfos[transfer.KaynakHesapIndex].HesapNo;
                        int kaynakDovizCinsi = accountInfos[transfer.KaynakHesapIndex].DovizCins;

                        DTODovizHareket dovizHareket = new DTODovizHareket
                        {
                            KaynakHesapNo = kaynakHesapNo,
                            HedefHesapNo = transfer.HedefHesapNo,
                            KaynakDovizCinsi = kaynakDovizCinsi,
                            HedefDovizCinsi = hedefDovizCinsi,
                            Miktar = transfer.Miktar
                        };

                        if (kaynakDovizCinsi == hedefDovizCinsi)
                        {
                            bool isOwnAccount = IsOwnAccount(accountInfos, transfer.HedefHesapNo);
                            if (isOwnAccount)
                            {
                                Console.WriteLine("Hedef hesap kendi hesabınız. Havale işlemi gerçekleştirilemez.");
                            }
                            else
                            {
                                DTOAccountInfo accountInfo = new DTOAccountInfo()
                                {
                                    HesapNo = kaynakHesapNo
                                };

                                bool transferBasarili = account.Havale(dovizHareket, accountInfo);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Kaynak hesap ve hedef hesap döviz cinsleri uyuşmuyor. Havale işlemi gerçekleştirilemedi.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Geçersiz hesap indexi. Tekrar deneyin.");
                    }
                }
                else
                {
                    Console.WriteLine("Hedef hesap numarası bankamıza ait değil lütfen EFT işlemi gerçekleştiriniz.");
                }
            }
            catch (Exception ex)
            {
                LogAndHandleError(ex);
            }
        }

        public void EFT(int musteriId, int kaynakHesapIndex, long hedefHesapNo, decimal transferMiktar)
        {
            try
            {
                DTOAccountInfo dTOAccount = new DTOAccountInfo();
                List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccount);

                if (kaynakHesapIndex >= 0 && kaynakHesapIndex < accountInfos.Count)
                {
                    long kaynakHesapNo = accountInfos[kaynakHesapIndex].HesapNo;
                    int kaynakDovizCinsi = accountInfos[kaynakHesapIndex].DovizCins;

                    if (kaynakDovizCinsi == (hedefHesapNo))
                    {
                        bool isOwnAccount = IsOwnAccount(accountInfos, hedefHesapNo);
                        if (isOwnAccount)
                        {
                            Console.WriteLine("Hedef hesap kendi hesabınız. EFT işlemi gerçekleştirilemez.");
                        }
                        else
                        {
                            bool transferBasarili = transferDB.EFT(kaynakHesapNo, hedefHesapNo, transferMiktar);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Kaynak hesap ve hedef hesap döviz cinsleri uyuşmuyor. EFT işlemi gerçekleştirilemedi.");
                    }
                }
                else
                {
                    Console.WriteLine("Geçersiz hesap indexi. Tekrar deneyin.");
                }
            }
            catch (Exception ex)
            {
                LogAndHandleError(ex);
            }
        }

        private bool IsOwnAccount(List<DTOAccountInfo> accountInfos, long hesapNo)
        {
            foreach (DTOAccountInfo accountInfo in accountInfos)
            {
                if (accountInfo.HesapNo == hesapNo)
                {
                    return true;
                }
            }
            return false;
        }

        private int GetDovizCinsiFromHesapNo(long hesapNo)
        {
            string dovizKod = hesapNo.ToString().Substring(0, 1);

            switch (dovizKod)
            {
                case "1":
                    return 1;
                case "2":
                    return 2;
                case "3":
                    return 3;
                case "4":
                    return 4;
                case "5":
                    return 5;
                default:
                    throw new Exception("Geçersiz döviz kodu");
            }
        }

        private void LogAndHandleError(Exception ex)
        {
            // Log the error to the database using the ErrorLoggerDB
            MethodBase method = MethodBase.GetCurrentMethod();
            FabrieBank.DAL.DataAccessLayer dataAccessLayer = new DAL.DataAccessLayer();
            dataAccessLayer.LogError(ex, method.ToString());

            // Handle the error (display a user-friendly message, rollback transactions, etc.)
            Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
        }

        public void PrintAccountList(List<DTOAccountInfo> accountInfos)
        {
            Console.WriteLine("Hesaplarınız:");
            for (int i = 0; i < accountInfos.Count; i++)
            {
                Console.WriteLine($"[{i}] Hesap No: {accountInfos[i].HesapNo}");
                Console.WriteLine($"Bakiye: {accountInfos[i].Bakiye}");
                Console.WriteLine($"Doviz Cinsi: {accountInfos[i].DovizCins}");
                Console.WriteLine("==============================");
            }
        }
    }
}
