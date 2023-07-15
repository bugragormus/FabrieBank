//using System;
//using FabrieBank.Common.Enums;
//using FabrieBank.Entity;

//namespace FabrieBank
//{
//    public class ATM
//    {
//        private readonly LogInDB logInDB;
//        private readonly AccInfoDB accInfoDB;
//        private readonly TransactionDB transactionDB;

//        public ATM()
//        {
//            logInDB = new LogInDB();
//            accInfoDB = new AccInfoDB();
//            transactionDB = new TransactionDB();
//        }

//        public void StartATM()
//        {
//            Console.WriteLine("==============================================");
//            Console.WriteLine("              ATM İşlemleri");
//            Console.WriteLine("==============================================");

//            Console.WriteLine("TCKN girin:");
//            Console.Write(">>> ");
//            long tckn = long.Parse(Console.ReadLine());

//            Console.WriteLine("Şifre girin:");
//            Console.Write(">>> ");
//            int sifre = int.Parse(Console.ReadLine());

//            bool loginSuccess = logInDB.LogIn(tckn, sifre);
//            if (!loginSuccess)
//            {
//                Console.WriteLine("Giriş başarısız. Lütfen geçerli bir TCKN ve şifre girin.");
//                return;
//            }

//            Console.WriteLine("Müşteri ID'sini girin:");
//            Console.Write(">>> ");
//            int musteriId = int.Parse(Console.ReadLine());

//            DTOAccountInfo accountInfo = accInfoDB.AccInfo(musteriId);
//            if (accountInfo.MusteriId == 0)
//            {
//                Console.WriteLine("Geçerli bir müşteri ID girin.");
//                return;
//            }

//            Console.WriteLine("İşlem Seçin:");
//            Console.WriteLine("1. Para Yatırma");
//            Console.WriteLine("2. Para Çekme");
//            Console.WriteLine("==============================================");
//            Console.Write(">>> ");
//            string choice = Console.ReadLine();

//            switch (choice)
//            {
//                case "1":
//                    DepositMoney(accountInfo);
//                    break;
//                case "2":
//                    WithdrawMoney(accountInfo);
//                    break;
//                default:
//                    Console.WriteLine("Geçersiz bir seçim yaptınız.");
//                    break;
//            }
//        }

//        private void DepositMoney(DTOAccountInfo accountInfo)
//        {
//            Console.WriteLine("Yatırılacak miktarı girin:");
//            Console.Write(">>> ");
//            long amount = long.Parse(Console.ReadLine());

//            transactionDB.Deposit(accountInfo.HesapNo, amount);

//            Console.WriteLine("Para yatırma işlemi tamamlandı.");
//            Console.WriteLine("Yeni bakiye: {0}", accountInfo.Bakiye + amount);
//        }

//        private void WithdrawMoney(DTOAccountInfo accountInfo)
//        {
//            Console.WriteLine("Çekilecek miktarı girin:");
//            Console.Write(">>> ");
//            long amount = long.Parse(Console.ReadLine());

//            if (amount > accountInfo.Bakiye)
//            {
//                Console.WriteLine("Yetersiz bakiye. İşlem gerçekleştirilemedi.");
//                return;
//            }

//            transactionDB.Withdraw(accountInfo.HesapNo, amount);

//            Console.WriteLine("Para çekme işlemi tamamlandı.");
//            Console.WriteLine("Yeni bakiye: {0}", accountInfo.Bakiye - amount);
//        }
//    }
//}
