using System;

namespace FabrieBank
{
    public class ATMMenu
    {
        private int musteriId;
        private FabrieBank.Entity.ATM atm;

        public ATMMenu(int musteriId)
        {
            this.musteriId = musteriId;
            atm = new FabrieBank.Entity.ATM();
        }

        public void ShowMenu()
        {
            string choice;

            do
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine("ATM İŞLEMLERİ");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Para Yatırma");
                Console.WriteLine("2. Para Çekme");
                Console.WriteLine("3. Üst Menü");
                Console.WriteLine("==============================");
                Console.Write("Seçiminizi yapın (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ParaYatirma();
                        break;
                    case "2":
                        ParaCekme();
                        break;
                    case "3":
                        Console.WriteLine("ATM'den çıkış yapıldı.");
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim. Tekrar deneyin.");
                        break;
                }

                Console.WriteLine("==============================\n");
            } while (choice != "3");
        }

        private void ParaYatirma()
        {
            Console.WriteLine("\nPara yatırmak istediğiniz hesap numarasını girin: ");
            Console.Write(">>> ");
            long hesapNo = long.Parse(Console.ReadLine());

            Console.WriteLine("\nYatırmak istediğiniz miktarı girin: ");
            Console.Write(">>> ");
            long bakiye = long.Parse(Console.ReadLine());

            atm.ParaYatirma(hesapNo, bakiye);
        }

        private void ParaCekme()
        {
            Console.WriteLine("\nPara çekmek istediğiniz hesap numarasını girin: ");
            Console.Write(">>> ");
            long hesapNo = long.Parse(Console.ReadLine());

            Console.WriteLine("\nÇekmek istediğiniz miktarı girin: ");
            Console.Write(">>> ");
            long bakiye = long.Parse(Console.ReadLine());

            atm.ParaCekme(hesapNo, bakiye);
        }
    }
}
