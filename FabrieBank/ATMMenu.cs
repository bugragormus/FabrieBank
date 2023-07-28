using System.Reflection;
using FabrieBank.Entity;
using FabrieBank.BLL;

namespace FabrieBank
{
    public class ATMMenu
    {
        private int musteriId;
        private BAccount atm;

        public ATMMenu(int musteriId)
        {
            this.musteriId = musteriId;
            atm = new BAccount();
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
            } while (choice != "3");
        }

        private void ParaYatirma()
        {
            try
            {
                Console.WriteLine("\nPara yatırmak istediğiniz hesap numarasını girin: ");
                Console.Write(">>> ");
                long hesapNo = long.Parse(Console.ReadLine());

                Console.WriteLine("\nYatırmak istediğiniz miktarı girin: ");
                Console.Write(">>> ");
                decimal bakiye = decimal.Parse(Console.ReadLine());

                atm.Deposit(hesapNo, bakiye);
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                FabrieBank.DAL.DataAccessLayer dataAccessLayer = new DAL.DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        private void ParaCekme()
        {
            try
            {
                Console.WriteLine("\nPara çekmek istediğiniz hesap numarasını girin: ");
                Console.Write(">>> ");
                long hesapNo = long.Parse(Console.ReadLine());

                Console.WriteLine("\nÇekmek istediğiniz miktarı girin: ");
                Console.Write(">>> ");
                decimal bakiye = decimal.Parse(Console.ReadLine());

                atm.Withdraw(hesapNo, bakiye);
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                FabrieBank.DAL.DataAccessLayer dataAccessLayer = new DAL.DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }
    }
}
