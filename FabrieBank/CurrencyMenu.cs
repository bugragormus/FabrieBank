using FabrieBank.Common;
using FabrieBank.Entity;
using FabrieBank.BLL;
using FabrieBank.Common.Enums;

namespace FabrieBank
{
    public class CurrencyMenu
    {

        public CurrencyMenu()
        {
        }

        public void ShowMenu()
        {
            string choice;
            do
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine("CURRENCY RATES");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Todays Rates");
                Console.WriteLine("2. Another Days Rates");
                Console.WriteLine("3. Upper Menu");
                Console.WriteLine("==============================");
                Console.Write("Make a choice (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        TodaysRates();
                        break;
                    case "2":
                        CustomDayRates();
                        break;
                    case "3":
                        Console.WriteLine("\nPara transferinden çıkış yapıldı.\n");
                        break;
                    default:
                        Console.WriteLine("\nGeçersiz seçim. Tekrar deneyin.\n");
                        break;
                }
            } while (choice != "3");
        }

        private void TodaysRates()
        {
            CurrencyTable currencyTable = new CurrencyTable();
            currencyTable.DisplayCurrencyRatesTable(EnumDovizCinsleri.DovizCinsleri.TRY);
        }

        private void CustomDayRates()
        {
        }
    }
}