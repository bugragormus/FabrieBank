using FabrieBank.Common;
using FabrieBank.Entity;
using FabrieBank.BLL;

namespace FabrieBank
{
    public class TransferMenu
    {
        private int musteriId;
        private TransactionLogic transactionLogic;
        private AccInfoDB accInfoDB;

        public TransferMenu(int musteriId)
        {
            this.musteriId = musteriId;
            transactionLogic = new TransactionLogic();
            accInfoDB = new AccInfoDB();
        }

        public void ShowMenu()
        {
            List<DTOAccountInfo> accountInfos = accInfoDB.AccInfo(musteriId);

            string choice;
            do
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine("PARA TRANSFERİ");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Hesaplarım Arası Transfer");
                Console.WriteLine("2. Başka Hesaba Havale");
                Console.WriteLine("3. Başka Hesaba EFT");
                Console.WriteLine("4. Üst Menü");
                Console.WriteLine("==============================");
                Console.Write("Seçiminizi yapın (1-4): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HesaplarArasiTransfer(accountInfos);
                        break;
                    case "2":
                        Havale(accountInfos);
                        break;
                    case "3":
                        EFT(accountInfos);
                        break;
                    case "4":
                        Console.WriteLine("Para transferinden çıkış yapıldı.");
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim. Tekrar deneyin.");
                        break;
                }
            } while (choice != "4");
        }

        private void Havale(List<DTOAccountInfo> accountInfos)
        {
            Console.WriteLine("\nHangi hesaptan para çekmek istiyorsunuz?");
            transactionLogic.PrintAccountList(accountInfos);

            Console.Write("Kaynak Hesap Indexi: ");
            int kaynakHesapIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nHavale yapmak istediğiniz hesap numarasını girin: ");
            Console.Write("Hedef Hesap Numarası: ");
            long hedefHesapNo = long.Parse(Console.ReadLine());

            Console.WriteLine("\nTransfer etmek istediğiniz miktarı girin: ");
            decimal transferMiktar = decimal.Parse(Console.ReadLine());

            transactionLogic.Havale(musteriId, kaynakHesapIndex, hedefHesapNo, transferMiktar);
        }

        private void EFT(List<DTOAccountInfo> accountInfos)
        {
            Console.WriteLine("\nHangi hesaptan para çekmek istiyorsunuz?");
            transactionLogic.PrintAccountList(accountInfos);

            Console.Write("Kaynak Hesap Indexi: ");
            int kaynakHesapIndex = int.Parse(Console.ReadLine());

            Console.WriteLine("\nEFT yapmak istediğiniz hesap numarasını girin: ");
            Console.Write("Hedef Hesap Numarası: ");
            long hedefHesapNo = long.Parse(Console.ReadLine());

            Console.WriteLine("\nTransfer etmek istediğiniz miktarı girin: ");
            decimal transferMiktar = decimal.Parse(Console.ReadLine());

            transactionLogic.EFT(musteriId, kaynakHesapIndex, hedefHesapNo, transferMiktar);
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

            transactionLogic.HesaplarArasiTransfer(musteriId, kaynakHesapIndex, hedefHesapIndex, transferMiktar);
        }
    }
}