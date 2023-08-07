using System.Text.RegularExpressions;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

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
                string? customerName = Console.ReadLine();
                customerName = char.ToUpper(customerName[0]) + customerName.Substring(1);
                while (string.IsNullOrEmpty(customerName) || customerName.Length <= 2)
                {
                    Console.WriteLine("Name must be a string and longer than 2 characters.");
                    Console.Write("Customer Name: ");
                    customerName = Console.ReadLine();
                }

                Console.WriteLine("\n==============================================");
                Console.Write("Customer Lastname: ");
                string? customerLastname = Console.ReadLine();
                customerLastname = char.ToUpper(customerLastname[0]) + customerLastname.Substring(1);
                while (string.IsNullOrEmpty(customerLastname) || customerLastname.Length <= 2)
                {
                    Console.WriteLine("Lastname must be a string and longer than 2 characters.");
                    Console.Write("Customer Lastname: ");
                    customerLastname = Console.ReadLine();
                }

                Console.WriteLine("\n==============================================");
                Console.Write("Customer TCKN: ");
                long customerTckn;
                while (!long.TryParse(Console.ReadLine(), out customerTckn) || customerTckn.ToString().Length != 11)
                {
                    Console.WriteLine("Invalid TCKN. Please enter a 11-digit TCKN:");
                }


                Console.WriteLine("\n==============================================");
                Console.Write("Customer Password: ");
                int customerPassword;
                while (!int.TryParse(GetMaskedInput(), out customerPassword) || customerPassword.ToString().Length != 4)
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
                long customerCellNo;
                while (!long.TryParse(Console.ReadLine(), out customerCellNo) || customerCellNo.ToString().Length != 10)
                {
                    Console.WriteLine("Invalid phone number. Please enter a 10-digit phone number:");
                }

                Console.WriteLine("\n==============================================");
                Console.Write("Customer Email: ");
                string customerEmail = Console.ReadLine();

                while (!IsValidEmail(customerEmail))
                {
                    Console.WriteLine("Invalid email address. Please enter a valid email address:");
                    customerEmail = Console.ReadLine();
                }

                DTOCustomer customer = new DTOCustomer
                {
                    Name = customerName,
                    Lastname = customerLastname,
                    Tckn = customerTckn,
                    Password = customerPassword,
                    CellNo = customerCellNo,
                    Email = customerEmail
                };

                ECustomer createCustomerDB = new ECustomer();
                createCustomerDB.CreateCustomer(customer);
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
    }
}