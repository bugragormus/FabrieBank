using System.Reflection;
using System.Text.RegularExpressions;
using FabrieBank.DAL.Common.DTOs;

namespace FabrieBank
{
    public class CreateMusteri
    {
        public void CreateMusteriM()
        {
            try
            {
                Console.WriteLine("\n==============================================");
                Console.Write("Customer Name: ");
                string? musteriAd = Console.ReadLine();
                musteriAd = char.ToUpper(musteriAd[0]) + musteriAd.Substring(1);
                while (string.IsNullOrEmpty(musteriAd) || musteriAd.Length <= 2)
                {
                    Console.WriteLine("Name must be a string and longer than 2 characters.");
                    Console.Write("Customer Name: ");
                    musteriAd = Console.ReadLine();
                }

                Console.WriteLine("\n==============================================");
                Console.Write("Customer Lastname: ");
                string? musteriSoyad = Console.ReadLine();
                musteriSoyad = char.ToUpper(musteriSoyad[0]) + musteriSoyad.Substring(1);
                while (string.IsNullOrEmpty(musteriAd) || musteriAd.Length <= 2)
                {
                    Console.WriteLine("Lastname must be a string and longer than 2 characters.");
                    Console.Write("Customer Lastname: ");
                    musteriAd = Console.ReadLine();
                }

                Console.WriteLine("\n==============================================");
                Console.Write("Customer TCKN: ");
                long musteriTckn;
                while (!long.TryParse(Console.ReadLine(), out musteriTckn) || musteriTckn.ToString().Length != 11)
                {
                    Console.WriteLine("Invalid TCKN. Please enter a 11-digit TCKN:");
                }


                Console.WriteLine("\n==============================================");
                Console.Write("Customer Password: ");
                int musteriSifre;
                while (!int.TryParse(GetMaskedInput(), out musteriSifre) || musteriSifre.ToString().Length != 4)
                {
                    Console.WriteLine("Invalid password. Please enter a 4-digit password:");
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

                Console.WriteLine("\n\n==============================================");
                Console.Write("Customer Cell Number: ");
                long musteriTelNo;
                while (!long.TryParse(Console.ReadLine(), out musteriTelNo) || musteriTelNo.ToString().Length != 10)
                {
                    Console.WriteLine("Invalid phone number. Please enter a 10-digit phone number:");
                }

                Console.WriteLine("\n==============================================");
                Console.Write("Customer Email: ");
                string musteriEmail = Console.ReadLine();

                while (!IsValidEmail(musteriEmail))
                {
                    Console.WriteLine("Invalid email address. Please enter a valid email address:");
                    musteriEmail = Console.ReadLine();
                }

                DTOCustomer customer = new DTOCustomer
                {
                    Ad = musteriAd,
                    Soyad = musteriSoyad,
                    Tckn = musteriTckn,
                    Sifre = musteriSifre,
                    TelNo = musteriTelNo,
                    Email = musteriEmail
                };

                //CreateCustomerDB createCustomerDB = new CreateCustomerDB();
                //createCustomerDB.CreateCustomer(customer);
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