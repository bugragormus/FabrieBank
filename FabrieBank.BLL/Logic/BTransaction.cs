using System.Reflection;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank.BLL.Logic
{
    public class BTransaction
    {
        private EAccountInfo eAccount;
        private TransferDB transferDB;

        public BTransaction()
        {
            eAccount = new EAccountInfo();
            transferDB = new TransferDB();
        }

        public void HesaplarArasiTransfer(int musteriId, int kaynakHesapIndex, int hedefHesapIndex, decimal transferMiktar)
        {
            try
            {
                DTOAccountInfo dTOAccount = new DTOAccountInfo();
                List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccount);

                if (kaynakHesapIndex >= 0 && kaynakHesapIndex < accountInfos.Count && hedefHesapIndex >= 0 && hedefHesapIndex < accountInfos.Count)
                {
                    long kaynakHesapNo = accountInfos[kaynakHesapIndex].HesapNo;
                    long hedefHesapNo = accountInfos[hedefHesapIndex].HesapNo;
                    int kaynakDovizCinsi = accountInfos[kaynakHesapIndex].DovizCins;
                    int hedefDovizCinsi = accountInfos[hedefHesapIndex].DovizCins;

                    if (KaynakVeHedefDovizCinsleriUyusuyorMu(kaynakHesapNo, hedefHesapNo, kaynakDovizCinsi, hedefDovizCinsi))
                    {
                        DTODovizHareket dovizHareket = new DTODovizHareket
                        {
                            KaynakHesapNo = kaynakHesapNo,
                            HedefHesapNo = hedefHesapNo,
                            DovizCinsi = kaynakDovizCinsi,
                            Miktar = transferMiktar
                        };

                        bool transferBasarili = transferDB.HesaplarArasiTransfer(dovizHareket.KaynakHesapNo, dovizHareket.HedefHesapNo, dovizHareket.Miktar);
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

        public void Havale(int musteriId, int kaynakHesapIndex, long hedefHesapNo, decimal transferMiktar)
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
                            Console.WriteLine("Hedef hesap kendi hesabınız. Havale işlemi gerçekleştirilemez.");
                        }
                        else
                        {
                            bool transferBasarili = transferDB.Havale(kaynakHesapNo, hedefHesapNo, transferMiktar);
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

        private bool KaynakVeHedefDovizCinsleriUyusuyorMu(long kaynakHesapNo, long hedefHesapNo, int kaynakDovizCinsi, int hedefDovizCinsi)
        {
            string kaynakDovizKod = kaynakHesapNo.ToString().Substring(0, 1);
            string hedefDovizKod = hedefHesapNo.ToString().Substring(0, 1);

            return kaynakDovizKod == hedefDovizKod;
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
