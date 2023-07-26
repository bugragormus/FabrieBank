using System.Reflection;
using FabrieBank.Common;
using FabrieBank.Common.DTOs;
using FabrieBank.Common.Enums;
using FabrieBank.Entity;
using Npgsql;

namespace FabrieBank.BLL
{
	public class AccountLogic
	{
        private AccInfoDB accInfoDB;

        public AccountLogic()
        {
            accInfoDB = new AccInfoDB();
        }

        public string GetDovizCinsi(EnumDovizCinsleri.DovizCinsleri dovizCins)
        {
            switch (dovizCins)
            {
                case EnumDovizCinsleri.DovizCinsleri.TRY:
                    return "Türk Lirası (TRY)";
                case EnumDovizCinsleri.DovizCinsleri.USD:
                    return "Amerikan Doları (USD)";
                case EnumDovizCinsleri.DovizCinsleri.EUR:
                    return "Euro (EUR)";
                case EnumDovizCinsleri.DovizCinsleri.GBP:
                    return "Gram Altın (GBP)";
                case EnumDovizCinsleri.DovizCinsleri.CHF:
                    return "Gram Gümüş (CHF)";
                default:
                    return string.Empty;
            }
        }

        public void AccountLogicM(DTOCustomer customer)
        {
            AccInfoDB accInfoDB1 = new AccInfoDB();
            int musteriId = customer.MusteriId;
            List<DTOAccountInfo> accountInfos = accInfoDB1.AccInfo(musteriId);

            foreach (DTOAccountInfo accountInfo in accountInfos)
            {
                string dovizCinsi = GetDovizCinsi(accountInfo.DovizCins);

                Console.WriteLine($"Hesap No: {accountInfo.HesapNo}");
                Console.WriteLine($"Bakiye: {accountInfo.Bakiye}");
                Console.WriteLine($"DovizCins: {dovizCinsi}");
                Console.WriteLine($"HesapAdi: {accountInfo.HesapAdi}");
                Console.WriteLine("==============================\n");
            }

            AccInfoDB accInfoDB = new AccInfoDB();
            accInfoDB.AccInfo(musteriId);
        }

        public void HesapSil()
        {
            Console.WriteLine("\nSilmek istediğiniz hesap numarasını girin: ");
            long hesapNo = long.Parse(Console.ReadLine());
            _ = accInfoDB.HesapSil(hesapNo);
        }

        public void Deposit(long hesapNo, decimal bakiye)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string functionName = "func_ReadAccountInfo";

                    string sqlSelect = $"SELECT * FROM {functionName}(@hesapNo)";
                    string sqlUpdate = "UPDATE public.Hesap SET Bakiye = Bakiye + @bakiye WHERE HesapNo = @hesapNo";

                    using (NpgsqlCommand commandSelect = new NpgsqlCommand(sqlSelect, connection))
                    {
                        commandSelect.Parameters.AddWithValue("@hesapNo", hesapNo);

                        decimal eskiBakiye = Convert.ToDecimal(commandSelect.ExecuteScalar());
                        decimal yeniBakiye = eskiBakiye + bakiye;

                        using (NpgsqlCommand commandUpdate = new NpgsqlCommand(sqlUpdate, connection))
                        {
                            commandUpdate.Parameters.AddWithValue("@bakiye", bakiye);
                            commandUpdate.Parameters.AddWithValue("@hesapNo", hesapNo);

                            int rowsAffected = commandUpdate.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {

                                // Log the successful deposit
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    AccountNumber = hesapNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Success,
                                    Amount = yeniBakiye - eskiBakiye,
                                    OldBalance = eskiBakiye,
                                    NewBalance = yeniBakiye,
                                    Timestamp = DateTime.Now
                                };

                                LogTransaction(transactionLog);

                                Console.WriteLine("\nPara yatırma işlemi başarılı.");
                                Console.WriteLine($"Eski bakiye: {eskiBakiye}");
                                Console.WriteLine($"Yeni bakiye: {yeniBakiye}");
                            }
                            else
                            {

                                // Log the failed deposit
                                DTOTransactionLog transactionLog = new DTOTransactionLog
                                {
                                    AccountNumber = hesapNo,
                                    TransactionType = EnumTransactionType.Deposit,
                                    TransactionStatus = EnumTransactionStatus.Failed,
                                    Amount = yeniBakiye - eskiBakiye,
                                    OldBalance = eskiBakiye,
                                    NewBalance = yeniBakiye,
                                    Timestamp = DateTime.Now
                                };

                                LogTransaction(transactionLog);

                                Console.WriteLine("\nPara yatırma işlemi başarısız.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine("An error occurred while performing ParaYatirma operation. Please try again later.");
            }
        }

        public void Withdraw(long hesapNo, decimal bakiye)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(database.ConnectionString))
                {
                    connection.Open();

                    string sqlSelect = "SELECT Bakiye FROM public.Hesap WHERE HesapNo = @hesapNo";
                    string sqlUpdate = "UPDATE public.Hesap SET Bakiye = @bakiye WHERE HesapNo = @hesapNo";

                    using (NpgsqlCommand commandSelect = new NpgsqlCommand(sqlSelect, connection))
                    {
                        commandSelect.Parameters.AddWithValue("@hesapNo", hesapNo);

                        decimal eskiBakiye = Convert.ToDecimal(commandSelect.ExecuteScalar());
                        decimal yeniBakiye = eskiBakiye - bakiye;

                        if (yeniBakiye >= 0)
                        {
                            using (NpgsqlCommand commandUpdate = new NpgsqlCommand(sqlUpdate, connection))
                            {
                                commandUpdate.Parameters.AddWithValue("@bakiye", yeniBakiye);
                                commandUpdate.Parameters.AddWithValue("@hesapNo", hesapNo);

                                int rowsAffected = commandUpdate.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    // Log the successful withdrawal
                                    DTOTransactionLog transactionLog = new DTOTransactionLog
                                    {
                                        AccountNumber = hesapNo,
                                        TransactionType = EnumTransactionType.Withdrawal,
                                        TransactionStatus = EnumTransactionStatus.Success,
                                        Amount = eskiBakiye - yeniBakiye,
                                        OldBalance = eskiBakiye,
                                        NewBalance = yeniBakiye,
                                        Timestamp = DateTime.Now
                                    };

                                    LogTransaction(transactionLog);

                                    Console.WriteLine("\nPara çekme işlemi başarılı.");
                                    Console.WriteLine($"Eski bakiye: {eskiBakiye}");
                                    Console.WriteLine($"Yeni bakiye: {yeniBakiye}");
                                }
                                else
                                {

                                    // Log the failed withdraw
                                    DTOTransactionLog transactionLog = new DTOTransactionLog
                                    {
                                        AccountNumber = hesapNo,
                                        TransactionType = EnumTransactionType.Withdrawal,
                                        TransactionStatus = EnumTransactionStatus.Failed,
                                        Amount = eskiBakiye - yeniBakiye,
                                        OldBalance = eskiBakiye,
                                        NewBalance = yeniBakiye,
                                        Timestamp = DateTime.Now
                                    };

                                    LogTransaction(transactionLog);

                                    Console.WriteLine("\nPara çekme işlemi başarısız.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n\nYetersiz bakiye. Para çekme işlemi gerçekleştirilemedi.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error to the database using the ErrorLoggerDB
                MethodBase method = MethodBase.GetCurrentMethod();
                LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine("An error occurred while performing ParaCekme operation. Please try again later.");
            }
        }
    }
}

