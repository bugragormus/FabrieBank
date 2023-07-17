using System;
using System.Collections.Generic;
using FabrieBank.Common;
using FabrieBank.Common.Enums;
using FabrieBank.DTO;
using FabrieBank.Entity;

namespace FabrieBank
{
    public class TransferMenu
    {
        private int musteriId;
        private AccInfoDB accInfoDB;
        private TransferDB transferDB;

        public TransferMenu(int musteriId)
        {
            this.musteriId = musteriId;
            accInfoDB = new AccInfoDB();
            transferDB = new TransferDB();
        }

        public void ShowMenu()
        {
            string choice;

            do
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine("PARA TRANSFERİ");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Hesaplarım Arası Transfer");
                Console.WriteLine("2. Başka Hesaba Havale/EFT");
                Console.WriteLine("3. Üst Menü");
                Console.WriteLine("==============================");
                Console.Write("Seçiminizi yapın (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HesaplarArasiTransfer();
                        break;
                    case "2":
                        HavaleEFT();
                        break;
                    case "3":
                        Console.WriteLine("Para transferinden çıkış yapıldı.");
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim. Tekrar deneyin.");
                        break;
                }

                Console.WriteLine("==============================\n");
            } while (choice != "3");
        }

        private void HesaplarArasiTransfer()
        {
            Console.WriteLine("\nHangi hesaptan para çekmek istiyorsunuz?");
            List<DTOAccountInfo> accountInfos = accInfoDB.AccInfo(musteriId);
            PrintAccountList(accountInfos);

            Console.Write("Kaynak Hesap Indexi: ");
            int kaynakHesapIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nHangi hesaba para aktarmak istiyorsunuz?");
            PrintAccountList(accountInfos);

            Console.Write("Hedef Hesap Indexi: ");
            int hedefHesapIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nTransfer etmek istediğiniz miktarı girin: ");
            long transferMiktar = long.Parse(Console.ReadLine());

            if (kaynakHesapIndex >= 0 && kaynakHesapIndex < accountInfos.Count && hedefHesapIndex >= 0 && hedefHesapIndex < accountInfos.Count)
            {
                long kaynakHesapNo = accountInfos[kaynakHesapIndex].HesapNo;
                long hedefHesapNo = accountInfos[hedefHesapIndex].HesapNo;
                EnumDovizCinsleri.DovizCinsleri kaynakDovizCinsi = accountInfos[kaynakHesapIndex].DovizCins;
                EnumDovizCinsleri.DovizCinsleri hedefDovizCinsi = accountInfos[hedefHesapIndex].DovizCins;

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


        private void HavaleEFT()
        {
            Console.WriteLine("\nHangi hesaptan para çekmek istiyorsunuz?");
            List<DTOAccountInfo> accountInfos = accInfoDB.AccInfo(musteriId);
            PrintAccountList(accountInfos);

            Console.Write("Kaynak Hesap Indexi: ");
            int kaynakHesapIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nHavale/EFT yapmak istediğiniz hesap numarasını girin: ");
            Console.Write("Hedef Hesap Numarası: ");
            long hedefHesapNo = long.Parse(Console.ReadLine());

            Console.WriteLine("\nTransfer etmek istediğiniz miktarı girin: ");
            long transferMiktar = long.Parse(Console.ReadLine());

            if (kaynakHesapIndex >= 0 && kaynakHesapIndex < accountInfos.Count)
            {
                long kaynakHesapNo = accountInfos[kaynakHesapIndex].HesapNo;
                EnumDovizCinsleri.DovizCinsleri kaynakDovizCinsi = accountInfos[kaynakHesapIndex].DovizCins;

                if (kaynakDovizCinsi == GetDovizCinsiFromHesapNo(hedefHesapNo))
                {
                    bool isOwnAccount = IsOwnAccount(accountInfos, hedefHesapNo);
                    if (isOwnAccount)
                    {
                        Console.WriteLine("Hedef hesap kendi hesabınız. Havale/EFT işlemi gerçekleştirilemez.");
                    }
                    else
                    {
                        bool transferBasarili = transferDB.HavaleEFT(kaynakHesapNo, hedefHesapNo, transferMiktar);
                        if (transferBasarili)
                        {
                            Console.WriteLine("Havale/EFT işlemi başarıyla gerçekleştirildi.");
                        }
                        else
                        {
                            Console.WriteLine("Havale/EFT işlemi gerçekleştirilemedi. Lütfen tekrar deneyin.");
                        }
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


        private bool KaynakVeHedefDovizCinsleriUyusuyorMu(long kaynakHesapNo, long hedefHesapNo, EnumDovizCinsleri.DovizCinsleri kaynakDovizCinsi, EnumDovizCinsleri.DovizCinsleri hedefDovizCinsi)
        {
            string kaynakDovizKod = kaynakHesapNo.ToString().Substring(0, 1);
            string hedefDovizKod = hedefHesapNo.ToString().Substring(0, 1);

            return kaynakDovizKod == hedefDovizKod;
        }

        private EnumDovizCinsleri.DovizCinsleri GetDovizCinsiFromHesapNo(long hesapNo)
        {
            string dovizKod = hesapNo.ToString().Substring(0, 1);

            switch (dovizKod)
            {
                case "1":
                    return EnumDovizCinsleri.DovizCinsleri.TL;
                case "2":
                    return EnumDovizCinsleri.DovizCinsleri.USD;
                case "3":
                    return EnumDovizCinsleri.DovizCinsleri.EUR;
                case "4":
                    return EnumDovizCinsleri.DovizCinsleri.GAU;
                case "5":
                    return EnumDovizCinsleri.DovizCinsleri.XAG;
                default:
                    throw new Exception("Geçersiz döviz kodu");
            }
        }

        private void PrintAccountList(List<DTOAccountInfo> accountInfos)
        {
            Console.WriteLine("Hesaplarınız:");
            for (int i = 0; i < accountInfos.Count; i++)
            {
                Console.WriteLine($"[{i}] Hesap No: {accountInfos[i].HesapNo}");
                Console.WriteLine($"Bakiye: {accountInfos[i].Bakiye}");
                Console.WriteLine("==============================");
            }
        }
    }
}