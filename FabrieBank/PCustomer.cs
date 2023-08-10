using System.Reflection;
using System.Text.RegularExpressions;
using FabrieBank.BLL.Logic;
using FabrieBank.DAL;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank
{
    public class PCustomer
    {
        private BCustomer bCustomer;
        private int customerId;

        public PCustomer(int customerId)
        {
            this.customerId = customerId;
            bCustomer = new BCustomer();
        }

        /// <summary>
        /// Gets input for customer creation
        /// </summary>
        public void CreateCustomer()
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

                string hashedPassword = DataAccessLayer.ComputeHash(customerPassword.ToString());

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
                    Password = hashedPassword,
                    CellNo = customerCellNo,
                    Email = customerEmail
                };

                bCustomer.CreateCustomer(customer);
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Gets input for Login
        /// </summary>
        public void LogIn()
        {
            try
            {

                Console.WriteLine("TCKN: ");
                Console.Write(">>> ");
                long tckn;
                while (!long.TryParse(Console.ReadLine(), out tckn) || tckn.ToString().Length != 11)
                {
                    Console.WriteLine("Invalid TCKN. Please enter a 11-digit TCKN:");
                }

                Console.WriteLine("Password: ");
                Console.Write(">>> ");
                int password;
                while (!int.TryParse(GetMaskedInput(), out password) || password.ToString().Length != 4)
                {
                    Console.WriteLine("Invalid password. Please enter a 4-digit password:");
                }

                string hashedPassword = DataAccessLayer.ComputeHash(password.ToString());

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

                DTOCustomer customer = bCustomer.LogIn(tckn, hashedPassword);

                if (customer != null)
                {
                    Console.Clear();
                    Console.WriteLine("******************************************************");
                    Console.WriteLine("*            |                         |             *");
                    Console.WriteLine("*            |    Login Successfull    |             *");
                    Console.WriteLine("*            V                         V             *");
                    Console.WriteLine("******************************************************\n");

                    Program program = new Program();
                    program.MainMenu(customer);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("**********************************************");
                    Console.WriteLine("*            !!! Login Failed !!!            *");
                    Console.WriteLine("*                                            *");
                    Console.WriteLine("*          !Wrong TCKN or Password!          *");
                    Console.WriteLine("**********************************************\n");
                }
            }
            catch (Exception ex)
            {
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Prints personel info
        /// </summary>
        /// <param name="customer"></param>
        public void PersonelInfo(DTOCustomer customer)
        {
            Console.Clear();
            Console.WriteLine("******************************************************");
            Console.WriteLine("*            |                         |             *");
            Console.WriteLine("*            |   Customer Information  |             *");
            Console.WriteLine("*            V                         V             *");
            Console.WriteLine("******************************************************\n");
            Console.WriteLine("Name         :" + customer.Name);
            Console.WriteLine("Last Name    :" + customer.Lastname);
            Console.WriteLine("TCKN         :" + customer.Tckn);
            Console.WriteLine("Phone Number :" + customer.CellNo);
            Console.WriteLine("Email        :" + customer.Email);
        }

        /// <summary>
        /// Gets input for updating personel info
        /// </summary>
        /// <param name="dTOCustomer"></param>
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
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Gets input for updating password
        /// </summary>
        /// <param name="dTOCustomer"></param>
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

                string currentHashedPassword = DataAccessLayer.ComputeHash(currentPassword.ToString());

                while (dTOCustomer.Password != currentHashedPassword)
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

                string newHashedPassword = DataAccessLayer.ComputeHash(newPassword.ToString());

                while (newHashedPassword == currentHashedPassword)
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
                    Password = newHashedPassword,
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
                MethodBase method = MethodBase.GetCurrentMethod();
                DataAccessLayer dataAccessLayer = new DataAccessLayer();
                dataAccessLayer.LogError(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Gets input for forgotten password
        /// </summary>
        /// <param name="customer"></param>
        public void ForgotPassword(DTOCustomer customer)
        {
            Console.Write("TCKN: ");
            long tckn;
            while (!long.TryParse(Console.ReadLine(), out tckn) || tckn.ToString().Length != 11)
            {
                Console.WriteLine("Invalid TCKN. Please enter a 11-digit TCKN:");
            }

            Console.Write("Email: ");
            string email = Console.ReadLine();

            while (!IsValidEmail(email))
            {
                Console.WriteLine("Invalid email address. Please enter a valid email address:");
                email = Console.ReadLine();
            }

            customer.Tckn = tckn;

            bCustomer.ForgotPassword(customer, email);
        }

        /// <summary>
        /// Email validation
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
    }
}