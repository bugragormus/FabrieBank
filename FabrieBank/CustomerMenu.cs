using System.Text.RegularExpressions;
using FabrieBank.BLL.Logic;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank
{
    public class CustomerMenu
    {
        private BCustomer bCustomer;
        private int customerId;
        private LogInDB customerInfoDB;

        public CustomerMenu(int customerId)
        {
            this.customerId = customerId;
            customerInfoDB = new LogInDB();
            bCustomer = new BCustomer();
        }

        public void UpdatePersonelInfo(DTOCustomer dTOCustomer)
        {
            try
            {
                Console.WriteLine("\nEnter New Phone Number: ");
                Console.Write(">>> ");
                long cellNo;
                while (!long.TryParse(Console.ReadLine(), out cellNo) || cellNo.ToString().Length != 10)
                {
                    Console.WriteLine("Invalid phone number. Please enter a 10-digit phone number:");
                }

                Console.WriteLine("\nEnter New Email: ");
                Console.Write(">>> ");
                string email = Console.ReadLine();

                while (!IsValidEmail(email))
                {
                    Console.WriteLine("Invalid email address. Please enter a valid email address:");
                    email = Console.ReadLine();
                }

                DTOCustomer customer = new DTOCustomer()
                {
                    CustomerId = customerId,
                    Name = dTOCustomer.Name,
                    Lastname = dTOCustomer.Lastname,
                    Password = dTOCustomer.Password,
                    CellNo = cellNo,
                    Email = email
                };

                bool updated = bCustomer.UpdatePersonelInfo(customer);

                if (updated)
                {
                    Console.WriteLine("\nPersonal information has been updated.\n");
                }
                else
                {
                    Console.WriteLine("\nAn error occurred while updating personal information.\n");
                }
            }
            catch (Exception ex)
            {
                EErrorLogger errorLogger = new EErrorLogger();
                errorLogger.LogAndHandleError(ex);
            }
        }

        public void ChangePassword(DTOCustomer dTOCustomer)
        {
            try
            {
                Console.WriteLine("Enter your current password:");
                Console.Write(">>> ");
                int currentPassword;
                while (!int.TryParse(GetMaskedInput(), out currentPassword) || currentPassword.ToString().Length != 4)
                {
                    Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                }

                while (dTOCustomer.Password != currentPassword)
                {
                    Console.WriteLine("\nThe current password and the entered password do not match, try again:");
                    Console.Write(">>> ");
                    while (!int.TryParse(GetMaskedInput(), out currentPassword) || currentPassword.ToString().Length != 4)
                    {
                        Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                    }
                }

                Console.WriteLine("\nPlease enter your new password:");
                Console.Write(">>> ");
                int newPassword;
                while (!int.TryParse(GetMaskedInput(), out newPassword) || newPassword.ToString().Length != 4)
                {
                    Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                }

                while (newPassword == currentPassword)
                {
                    Console.WriteLine("The new password cannot be the same as the old one. Please try again:");
                    Console.Write(">>> ");
                    while (!int.TryParse(GetMaskedInput(), out newPassword) || newPassword.ToString().Length != 4)
                    {
                        Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                    }
                }

                Console.WriteLine("\nPlease re-enter your new password:");
                Console.Write(">>> ");
                int newPassword2;
                while (!int.TryParse(GetMaskedInput(), out newPassword2) || newPassword2.ToString().Length != 4)
                {
                    Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                }

                while (newPassword != newPassword2)
                {
                    Console.WriteLine("Passwords do not match. Please re-enter the same password:");
                    Console.Write(">>> ");
                    while (!int.TryParse(GetMaskedInput(), out newPassword2) || newPassword2.ToString().Length != 4)
                    {
                        Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                    }
                }

                DTOCustomer customer = new DTOCustomer()
                {
                    CustomerId = customerId,
                    Name = dTOCustomer.Name,
                    Lastname = dTOCustomer.Lastname,
                    Password = newPassword2,
                    CellNo = dTOCustomer.CellNo,
                    Email = dTOCustomer.Email
                };

                bool updated = bCustomer.ChangePassword(customer);

                if (updated)
                {
                    Console.WriteLine("\n\nPassword has been updated.\n");
                }
                else
                {
                    Console.WriteLine("\nAn error occurred while updating password.\n");
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
                EErrorLogger errorLogger = new EErrorLogger();
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