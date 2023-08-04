using System.Text.RegularExpressions;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

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
                        UpdatePersonelInfo(customer);
                        break;
                    case "3":
                        ChangePassword(customer);
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

        private void UpdatePersonelInfo(DTOCustomer dTOCustomer)
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

                DTOCustomer customer = new DTOCustomer()
                {
                    MusteriId = musteriId,
                    Ad = dTOCustomer.Ad,
                    Soyad = dTOCustomer.Soyad,
                    Sifre = dTOCustomer.Sifre,
                    TelNo = telNo,
                    Email = email
                };

                bool updated = customerInfoDB.UpdatePersonelInfo(customer);

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
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }
        }

        public void ChangePassword(DTOCustomer dTOCustomer)
        {
            try
            {
                Console.WriteLine("Güncel şifrenizi giriniz: ");
                Console.Write(">>> ");
                int currentPassword;
                while (!int.TryParse(GetMaskedInput(), out currentPassword) || currentPassword.ToString().Length != 4)
                {
                    Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                }

                while (dTOCustomer.Sifre != currentPassword)
                {
                    Console.WriteLine("\nGüncel şifre ile girilen şifre uyuşmuyor yeniden deneyin:");
                    Console.Write(">>> ");
                    while (!int.TryParse(GetMaskedInput(), out currentPassword) || currentPassword.ToString().Length != 4)
                    {
                        Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                    }
                }

                Console.WriteLine("\nLütfen yeni şifrenizi girin: ");
                Console.Write(">>> ");
                int newPassword;
                while (!int.TryParse(GetMaskedInput(), out newPassword) || newPassword.ToString().Length != 4)
                {
                    Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                }

                while (newPassword == currentPassword)
                {
                    Console.WriteLine("Yeni şifre eskisi ile aynı olamaz. Lütfen yeniden deneyin:");
                    Console.Write(">>> ");
                    while (!int.TryParse(GetMaskedInput(), out newPassword) || newPassword.ToString().Length != 4)
                    {
                        Console.WriteLine("Geçersiz şifre. Lütfen 4 basamaklı bir şifre girin:");
                    }
                }

                Console.WriteLine("\nLütfen yeni şifrenizi tekrar girin: ");
                Console.Write(">>> ");
                int newPassword2;
                while (!int.TryParse(GetMaskedInput(), out newPassword2) || newPassword2.ToString().Length != 4)
                {
                    Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                }

                while (newPassword != newPassword2)
                {
                    Console.WriteLine("Şifreler eşleşmiyor. Lütfen aynı şifreyi tekrar girin:");
                    Console.Write(">>> ");
                    while (!int.TryParse(GetMaskedInput(), out newPassword2) || newPassword2.ToString().Length != 4)
                    {
                        Console.WriteLine("Geçersiz şifre. Lütfen 4 basamaklı bir şifre girin:");
                    }
                }

                DTOCustomer customer = new DTOCustomer()
                {
                    MusteriId = musteriId,
                    Ad = dTOCustomer.Ad,
                    Soyad = dTOCustomer.Soyad,
                    Sifre = newPassword2,
                    TelNo = dTOCustomer.TelNo,
                    Email = dTOCustomer.Email
                };

                bool updated = customerInfoDB.ChangePassword(customer);

                if (updated)
                {
                    Console.WriteLine("\n\nKullanıcı bilgileri güncellendi.\n");
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
                ErrorLoggerDB errorLogger = new ErrorLoggerDB();
                errorLogger.LogAndHandleError(ex);
            }
        }

        private static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
    }
}
