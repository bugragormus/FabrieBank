﻿using FabrieBank.Admin.DAL.DTO;
using FabrieBank.Admin.DAL.Entity;
using FabrieBank.BLL.Logic;
using FabrieBank.BLL.Service;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank.Admin.BLL.Logic
{
    public class BAdmin
    {
        private EAdmin eAdmin;
        private ECustomer eCustomer;
        private EAccountInfo eAccount;

        public BAdmin()
        {
            eAdmin = new EAdmin();
            eCustomer = new ECustomer();
            eAccount = new EAccountInfo();
        }

        /// <summary>
        /// Admin login logic
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public DTOAdmin LogIn(string nickname, string password)
        {
            DTOAdmin admin = eAdmin.ReadAdmin(new DTOAdmin { Nickname = nickname });

            if (admin != null && admin.Password == password)
            {
                return admin;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Customer status update logic
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool CustomerStatusUpdate(DTOCustomer customer)
        {
            eCustomer.UpdateCustomer(customer);
            return true;
        }

        /// <summary>
        /// ReadList and ReadById customer logic
        /// </summary>
        /// <param name="customer"></param>
        public void ListCustomers(DTOCustomer customer)
        {
            Console.WriteLine("\n");
            if (customer.Tckn != 0)
            {
                DTOCustomer dTOCustomer = eCustomer.ReadCustomer(customer);

                    Console.WriteLine($"ID      : {dTOCustomer.CustomerId}");
                    Console.WriteLine($"Name    : {dTOCustomer.Name}");
                    Console.WriteLine($"Lastname: {dTOCustomer.Lastname}");
                    Console.WriteLine($"TCKN    : {dTOCustomer.Tckn}");
                    Console.WriteLine($"Password: {dTOCustomer.Password}");
                    Console.WriteLine($"Cell No : {dTOCustomer.CellNo}");
                    Console.WriteLine($"Email   : {dTOCustomer.Email}");
                    Console.WriteLine($"Status  : {dTOCustomer.Status}");
                    Console.WriteLine("================================\n");

                if (dTOCustomer == null)
                {
                    Console.WriteLine("No customer was found that met the given parameters.");
                }
            }
            else
            {
                List<DTOCustomer> dTOCustomers = eCustomer.ReadListCustomer(customer);

                foreach (DTOCustomer dTOCustomer in dTOCustomers)
                {
                    Console.WriteLine($"ID      : {dTOCustomer.CustomerId}");
                    Console.WriteLine($"Name    : {dTOCustomer.Name}");
                    Console.WriteLine($"Lastname: {dTOCustomer.Lastname}");
                    Console.WriteLine($"TCKN    : {dTOCustomer.Tckn}");
                    Console.WriteLine($"Password: {dTOCustomer.Password}");
                    Console.WriteLine($"Cell No : {dTOCustomer.CellNo}");
                    Console.WriteLine($"Email   : {dTOCustomer.Email}");
                    Console.WriteLine($"Status  : {dTOCustomer.Status}");
                    Console.WriteLine("================================\n");
                }

                if (dTOCustomers.Count == 0)
                {
                    Console.WriteLine("No customer was found that met the given parameters.");
                }
            }
        }

        /// <summary>
        /// ReadList and ReadById account info logic
        /// </summary>
        /// <param name="accountInfo"></param>
        public void ListAccounts(DTOAccountInfo accountInfo)
        {
            Console.WriteLine("\n");
            if (accountInfo.AccountNo != 0)
            {
                DTOAccountInfo dTOAccountInfo = eAccount.ReadAccountInfo(accountInfo);

                Console.WriteLine($"Account No       : {dTOAccountInfo.AccountNo}");
                Console.WriteLine($"Balance          : {dTOAccountInfo.Balance}");
                Console.WriteLine($"Customer ID      : {dTOAccountInfo.CustomerId}");
                Console.WriteLine($"Currency Type    : {dTOAccountInfo.CurrencyType}");
                Console.WriteLine($"Account Name     : {dTOAccountInfo.AccountName}");
                Console.WriteLine("========================================\n");

                if (dTOAccountInfo == null)
                {
                    Console.WriteLine("No account was found that met the given parameters.");
                }
            }
            else
            {
                List<DTOAccountInfo> dTOAccountInfos = eAccount.ReadListAccountInfo(accountInfo);

                foreach (DTOAccountInfo dTOAccount in dTOAccountInfos)
                {
                    Console.WriteLine($"Account No       : {dTOAccount.AccountNo}");
                    Console.WriteLine($"Balance          : {dTOAccount.Balance}");
                    Console.WriteLine($"Customer ID      : {dTOAccount.CustomerId}");
                    Console.WriteLine($"Currency Type    : {dTOAccount.CurrencyType}");
                    Console.WriteLine($"Account Name     : {dTOAccount.AccountName}");
                    Console.WriteLine("========================================\n");
                }

                if (dTOAccountInfos.Count == 0)
                {
                    Console.WriteLine("No account was found that met the given parameters.");
                }
            }
        }
    }
}
