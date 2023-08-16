using FabrieBank.Admin.DAL.DTO;
using FabrieBank.Admin.PL;

namespace FabrieBank
{
    class Program
    {
        /// <summary>
        /// Main method
        /// </summary>
        public static void Main()
        {
            string? ch;

            for (; ; )
            {
                Console.WriteLine("=======================");
                Console.WriteLine("Welcome To Admin System");
                Console.WriteLine("=======================");
                Console.WriteLine("1.Login          2.Exit");
                Console.WriteLine("=======================");
                Console.Write(">>> ");
                ch = Console.ReadLine();

                DTOAdmin admin = new DTOAdmin();

                switch (ch)
                {
                    case "1":
                        PAdmin pAdmin = new PAdmin(admin.Nickname);
                        pAdmin.LogIn();
                        break;

                    case "2":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }

        /// <summary>
        /// Opening menu
        /// </summary>
        /// <param name="customer"></param>
        public void MainMenu(DTOAdmin admin)
        {
            string? ch;

            for (; ; )
            {
                Console.WriteLine("====================================================================================================");
                Console.WriteLine("                                           MAIN MENU                                                ");
                Console.WriteLine("====================================================================================================");
                Console.WriteLine("1.Customer Operations    2.Account Operations     3.Logs      4.Currencies     5.Fees      6.Log Out");
                Console.WriteLine("====================================================================================================");
                Console.Write(">>> ");
                ch = Console.ReadLine();
                Console.Clear();

                switch (ch)
                {
                    case "1":

                        CustomerMenu(admin);
                        break;

                    //case "2":

                    //    AccountMenu(admin);
                    //    break;

                    //case "3":

                    //    TransferMenu(admin.Nickname);
                    //    break;

                    //case "4":

                    //    CurrencyMenu(admin);
                    //    break;

                    case "6":

                        Main();
                        break;

                    default:

                        Console.WriteLine("Invalid Choice!");
                        break;
                }
            }
        }

        /// <summary>
        /// Menu for customer operations
        /// </summary>
        /// <param name="customer"></param>
        public void CustomerMenu(DTOAdmin admin)
        {
            string choice;

            if (admin.Nickname != null)
            {
                do
                {
                    Console.WriteLine("==============================");
                    Console.WriteLine("USER OPERATIONS");
                    Console.WriteLine("==============================");
                    Console.WriteLine("1. Customer Status Update");
                    Console.WriteLine("2. List Customers");
                    Console.WriteLine("3. Back to Main Menu");
                    Console.WriteLine("==============================");
                    Console.Write("Make your pick (1-3): ");
                    choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            PAdmin pAdmin = new PAdmin(admin.Nickname);
                            pAdmin.CustomerStatusUpdate();
                            break;
                        case "2":
                            PAdmin pAdmin1 = new PAdmin(admin.Nickname);
                            pAdmin1.ListCustomers(admin);
                            break;
                        case "3":
                            Console.WriteLine("\nExited from User Operations.\n");
                            break;
                        default:
                            Console.WriteLine("\nInvalid selection. Try again.\n");
                            break;
                    }
                } while (choice != "3");
            }
            else
            {
                Console.WriteLine("An Error Occured!");
                Main();
            }
        }
    }
}