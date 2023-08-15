using FabrieBank.DAL;
using FabrieBank.DAL.Entity;
using System.Reflection;
using FabrieBank.Admin.DAL.Common.DTO;
using FabrieBank.Admin.BLL.Logic;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Common.Enums;

namespace FabrieBank.Admin.PL
{
    public class PAdmin
    {
        private BAdmin bAdmin;
        private string nickname;

        public PAdmin(string nickname)
        {
            this.nickname = nickname;
            bAdmin = new BAdmin();
        }

        /// <summary>
        /// Admin login module
        /// </summary>
        public void LogIn()
        {
            try
            {
                Console.WriteLine("Nickname: ");
                Console.Write(">>> ");
                string? nickname = Console.ReadLine();
                while (string.IsNullOrEmpty(nickname) || nickname.Length <= 2)
                {
                    Console.WriteLine("Nickname must be a string and longer than 2 characters.");
                    Console.Write("Nickname: ");
                    nickname = Console.ReadLine();
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

                DTOAdmin admin = bAdmin.LogIn(nickname, hashedPassword);

                if (admin != null)
                {
                    Console.Clear();
                    Console.WriteLine("******************************************************");
                    Console.WriteLine("*            |                         |             *");
                    Console.WriteLine("*            |    Login Successfull    |             *");
                    Console.WriteLine("*            V                         V             *");
                    Console.WriteLine("******************************************************\n");

                    Program program = new Program();
                    program.MainMenu(admin);
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
                EErrorLog errorLog = new EErrorLog();
                errorLog.InsertErrorLog(ex, method.ToString());

                Console.WriteLine($"An error occurred while performing {method} operation. Please try again later.");
            }
        }

        /// <summary>
        /// Updates customer status
        /// </summary>
        public void CustomerStatusUpdate()
        {
            DTOCustomer customers = new DTOCustomer()
            {
                Status = (int)EnumCustomerStatus.Pending
            };

            ECustomer eCustomer = new ECustomer();
            List<DTOCustomer> dTOCustomers = eCustomer.ReadListCustomer(customers);

            Console.Clear();
            Console.WriteLine("\nPending Customers:\n");
            for (int i = 0; i < dTOCustomers.Count; i++)
            {
                Console.WriteLine($"[{i}] Customer ID: {dTOCustomers[i].CustomerId}");
                Console.WriteLine($"    Name       : {dTOCustomers[i].Name}");
                Console.WriteLine($"    Lastname   : {dTOCustomers[i].Lastname}");
                Console.WriteLine($"    TCKN       : {dTOCustomers[i].Tckn}");
                Console.WriteLine($"    Cell No    : {dTOCustomers[i].CellNo}");
                Console.WriteLine($"    Email      : {dTOCustomers[i].Email}");
                Console.WriteLine($"    Status     : {dTOCustomers[i].Status}");
                Console.WriteLine("==========================================\n");
            }
            Console.WriteLine("\nWhich customer would you like to update status?");
            Console.Write("Customer Index: ");
            int selectedCustomerIndex = int.Parse(Console.ReadLine());

            if (selectedCustomerIndex >= 0 && selectedCustomerIndex < dTOCustomers.Count)
            {
                Console.WriteLine($"\nCurrent status of selected customer is 'Pending'.");
                Console.WriteLine("\nDo you want to accept or decline? (Accept = 1, Decline = 2)");
                Console.Write(">>> ");
                string? ch = Console.ReadLine();

                switch (ch)
                {
                    case "1":

                        DTOCustomer acceptedCustomer = new DTOCustomer()
                        {
                            CustomerId = dTOCustomers[selectedCustomerIndex].CustomerId,
                            Name = dTOCustomers[selectedCustomerIndex].Name,
                            Lastname = dTOCustomers[selectedCustomerIndex].Lastname,
                            Password = dTOCustomers[selectedCustomerIndex].Password,
                            CellNo = dTOCustomers[selectedCustomerIndex].CellNo,
                            Email = dTOCustomers[selectedCustomerIndex].Email,
                            Status = (int)EnumCustomerStatus.Active
                        };

                        bool updated = bAdmin.CustomerStatusUpdate(acceptedCustomer);

                        if (updated)
                        {
                            Console.WriteLine("\nCustomer accepted.\n");
                        }
                        else
                        {
                            Console.WriteLine("\nAn error occurred while updating Customer Status.\n");
                        }

                        break;

                    case "2":

                        DTOCustomer declinedCustomer = new DTOCustomer()
                        {
                            CustomerId = dTOCustomers[selectedCustomerIndex].CustomerId,
                            Name = dTOCustomers[selectedCustomerIndex].Name,
                            Lastname = dTOCustomers[selectedCustomerIndex].Lastname,
                            Password = dTOCustomers[selectedCustomerIndex].Password,
                            CellNo = dTOCustomers[selectedCustomerIndex].CellNo,
                            Email = dTOCustomers[selectedCustomerIndex].Email,
                            Status = (int)EnumCustomerStatus.Inactive
                        };

                        bool updated1 = bAdmin.CustomerStatusUpdate(declinedCustomer);

                        if (updated1)
                        {
                            Console.WriteLine("\nCustomer declined.\n");
                        }
                        else
                        {
                            Console.WriteLine("\nAn error occurred while updating Customer Status.\n");
                        }

                        break;

                    default:

                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }
    }
}

