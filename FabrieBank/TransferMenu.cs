using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;
using FabrieBank.BLL.Logic;

namespace FabrieBank
{
    public class TransferMenu
    {
        private int musteriId;
        private BTransaction transactionLogic;
        private EAccountInfo eAccount;

        public TransferMenu(int musteriId)
        {
            this.musteriId = musteriId;
            transactionLogic = new BTransaction();
            eAccount = new EAccountInfo();
        }

        public void ShowMenu()
        {
            DTOAccountInfo dTOAccount = new DTOAccountInfo()
            {
                MusteriId = musteriId
            };
            List<DTOAccountInfo> accountInfos = eAccount.ReadListAccountInfo(dTOAccount);

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
                        HesaplarArasiTransfer(accountInfos);
                        break;
                    case "2":
                        HavaleEFT(accountInfos);
                        break;
                    case "3":
                        Console.WriteLine("Para transferinden çıkış yapıldı.");
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim. Tekrar deneyin.");
                        break;
                }
            } while (choice != "3");
        }

        private void HavaleEFT(List<DTOAccountInfo> accountInfos)
        {
            Console.WriteLine("\nHangi hesaptan para çekmek istiyorsunuz?");
            transactionLogic.PrintAccountList(accountInfos);

            Console.Write("Kaynak Hesap Indexi: ");
            int kaynakHesapIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nPara transferi yapmak istediğiniz hesap numarasını girin: ");
            Console.Write("Hedef Hesap Numarası: ");
            long hedefHesapNo = long.Parse(Console.ReadLine());

            Console.WriteLine("\nTransfer etmek istediğiniz miktarı girin: ");
            decimal transferMiktar = decimal.Parse(Console.ReadLine());

            DTOTransfer transfer = new DTOTransfer()
            {
                KaynakHesapIndex = kaynakHesapIndex,
                HedefHesapNo = hedefHesapNo,
                Miktar = transferMiktar
            };

            transactionLogic.HavaleEFT(musteriId, transfer);
        }

        private void HesaplarArasiTransfer(List<DTOAccountInfo> accountInfos)
        {
            Console.WriteLine("\nHangi hesaptan para çekmek istiyorsunuz?");
            transactionLogic.PrintAccountList(accountInfos);

            Console.Write("Kaynak Hesap Indexi: ");
            int kaynakHesapIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nHangi hesaba para aktarmak istiyorsunuz?");
            transactionLogic.PrintAccountList(accountInfos);

            Console.Write("Hedef Hesap Indexi: ");
            int hedefHesapIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nTransfer etmek istediğiniz miktarı girin: ");
            decimal transferMiktar = decimal.Parse(Console.ReadLine());

            DTOTransfer transfer = new DTOTransfer()
            {
                KaynakHesapIndex = kaynakHesapIndex,
                HedefHesapIndex = hedefHesapIndex,
                Miktar = transferMiktar
            };

            transactionLogic.HesaplarArasiTransfer(musteriId, transfer);
        }
    }
}