using System.Reflection;
using System.Text.RegularExpressions;
using FabrieBank.DAL.Common.DTOs;
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
                Console.WriteLine("1. Kişisel Bilgileri Görüntüle");
                Console.WriteLine("2. Kişisel Bilgileri Güncelle");
                Console.WriteLine("3. Şifre Değiştir");
                Console.WriteLine("4. Üst Menü");
                Console.WriteLine("==============================");
                Console.Write("Seçiminizi yapın (1-4): ");
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
                        ChangePassword();
                        break;
                    case "4":
                        Console.WriteLine("\nKullanıcı menüsünden çıkış yapıldı.\n");
                        break;
                    default:
                        Console.WriteLine("\nGeçersiz seçim. Tekrar deneyin.\n");
                        break;
                }
            } while (choice != "4");
        }

        private void UpdatePersonelInfo()
        {
            try
            {
                Console.WriteLine("\nYeni Telefon Numarasını Girin: ");
                Console.Write(">>> ");
                long telNo;
                while (!long.TryParse(Console.ReadLine(), out telNo) || telNo.ToString().Length != 10)
                {
                    Console.WriteLine("Invalid phone number. Please enter a 10-digit phone number:");
                }

                Console.WriteLine("\nYeni Email Adresini Girin: ");
                Console.Write(">>> ");
                string email = Console.ReadLine();

                while (!IsValidEmail(email))
                {
                    Console.WriteLine("Invalid email address. Please enter a valid email address:");
                    email = Console.ReadLine();
                }

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

        public void ChangePassword()
        {
            try
            {
                Console.WriteLine("Lütfen yeni şifrenizi girin: ");
                Console.Write(">>> ");
                int newPassword;
                while (!int.TryParse(GetMaskedInput(), out newPassword) || newPassword.ToString().Length != 4)
                {
                    Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                }

                bool updated = customerInfoDB.ChangePassword(musteriId, newPassword);

                if (updated)
                {
                    Console.WriteLine("\nKullanıcı bilgileri güncellendi.\n");
                }
                else
                {
                    Console.WriteLine("\nKullanıcı bilgileri güncellenirken bir hata oluştu.\n");
                }

                static string GetMaskedInput()
                {
                    string input = "";
                    ConsoleKeyInfo keyInfo;

                    do
                    {
                        keyInfo = Console.ReadKey(true);

                        if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
                        {
                            input += keyInfo.KeyChar;
                            Console.Write("*");
                        }
                        else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                        {
                            input = input.Substring(0, input.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                    while (keyInfo.Key != ConsoleKey.Enter);

                    return input;
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

        private static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
    }
}
