using System;
using System.Reflection;
using FabrieBank.Common;
using FabrieBank.Entity;

namespace FabrieBank
{
    public class CustomerMenu
    {
        private int musteriId;
        private LogInDB customerInfoDB;

        public CustomerMenu(int musteriId)
        {
            this.musteriId = musteriId;
            customerInfoDB = new LogInDB();
        }

        public void ShowMenu(DTOCustomer customer)
        {
            string choice;

            do
            {
                Console.WriteLine("==============================");
                Console.WriteLine("USER OPERATIONS");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Bilgileri Görüntüle");
                Console.WriteLine("2. Bilgileri Güncelle");
                Console.WriteLine("3. Üst Menü");
                Console.WriteLine("==============================");
                Console.Write("Seçiminizi yapın (1-3): ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":

                        PersonelInfo personelInfo = new PersonelInfo();
                        personelInfo.PersonelInfoM(customer);
                        break;
                    case "2":
                        UpdatePersonelInfo();
                        break;
                    case "3":
                        Console.WriteLine("\nKullanıcı menüsünden çıkış yapıldı.\n");
                        break;
                    default:
                        Console.WriteLine("\nGeçersiz seçim. Tekrar deneyin.\n");
                        break;
                }
            } while (choice != "3");
        }

        private void UpdatePersonelInfo()
        {
            try
            {
                Console.WriteLine("\nYeni Telefon Numarasını Girin: ");
                Console.Write(">>> ");
                long telNo = long.Parse(Console.ReadLine());

                Console.WriteLine("\nYeni Email Adresini Girin: ");
                Console.Write(">>> ");
                string email = Console.ReadLine();

                bool updated = customerInfoDB.UpdatePersonelInfo(musteriId, telNo, email);

                if (updated)
                {
                    Console.WriteLine("\nKullanıcı bilgileri güncellendi.\n");
                }
                else
                {
                    Console.WriteLine("\nKullanıcı bilgileri güncellenirken bir hata oluştu.\n");
                }
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
