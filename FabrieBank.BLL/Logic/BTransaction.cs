using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank.BLL.Logic
{
    public class BTransaction
    {
        private EAccountInfo eAccount;
        private BAccount account;
        private ErrorLoggerDB errorLogger;

        public BTransaction()
        {
            eAccount = new EAccountInfo();
            account = new BAccount();
            errorLogger = new ErrorLoggerDB();
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
                errorLogger.LogAndHandleError(ex);
            }
        }

        public void HavaleEFT(int musteriId, DTOTransfer transfer)
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

                                bool transferBasarili = account.HavaleEFT(dovizHareket, accountInfo);
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
                            Miktar = transfer.Miktar
                        };

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

                            bool transferBasarili = account.HavaleEFT(dovizHareket, accountInfo);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Geçersiz hesap indexi. Tekrar deneyin.");
                    }
                }
            }
            catch (Exception ex)
            {
                errorLogger.LogAndHandleError(ex);
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

        public void PrintAccountList(List<DTOAccountInfo> accountInfos)
        {
            try
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
            catch (Exception ex)
            {
                errorLogger.LogAndHandleError(ex);
            }
        }
    }
}
