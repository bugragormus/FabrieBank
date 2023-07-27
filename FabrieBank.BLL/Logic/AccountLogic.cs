using System.Reflection;
using System.Security.Principal;
using FabrieBank.Common;
using FabrieBank.Common.DTOs;
using FabrieBank.Common.Enums;
using FabrieBank.DAL;
using FabrieBank.Entity;
using Npgsql;

namespace FabrieBank.BLL
{
	public class AccountLogic
	{

        private DataAccessLayer dataAccessLayer;
        private NpgsqlConnectionStringBuilder database;
        private EAccountInfo eAccount;

        public AccountLogic()
        {
            eAccount = new EAccountInfo();
            dataAccessLayer = new DataAccessLayer();
            database = dataAccessLayer.CallDB();
        }

        public string GetDovizCinsi(int dovizCins)
        {
            switch (dovizCins)
            {
                case 1:
                    return "Türk Lirası (TRY)";
                case 2:
                    return "Amerikan Doları (USD)";
                case 3:
                    return "Euro (EUR)";
                case 4:
                    return "Gram Altın (GBP)";
                case 5:
                    return "Gram Gümüş (CHF)";
                default:
                    return string.Empty;
            }
        }

        public void AccountLogicM(DTOCustomer customer)
        {
            EAccountInfo eAccount1 = new EAccountInfo();
            DTOAccountInfo dTOAccount = new DTOAccountInfo()
            {
                MusteriId = customer.MusteriId,
            };
            List<DTOAccountInfo> accountInfos = eAccount1.ReadListAccountInfo(dTOAccount);

            foreach (DTOAccountInfo accountInfo in accountInfos)
            {
                string dovizCinsi = GetDovizCinsi(accountInfo.DovizCins);

                Console.WriteLine($"Hesap No: {accountInfo.HesapNo}");
                Console.WriteLine($"Bakiye: {accountInfo.Bakiye}");
                Console.WriteLine($"DovizCins: {accountInfo.DovizCins}");
                Console.WriteLine($"HesapAdi: {accountInfo.HesapAdi}");
                Console.WriteLine("==============================\n");
            }

            EAccountInfo accInfoDB = new EAccountInfo();
            accInfoDB.ReadListAccountInfo(dTOAccount);
            if (accountInfos.Count == 0)
            {
                Console.WriteLine("yok");
            }

        }

        public void HesapSil()
        {
            DTOAccountInfo dTOAccount = new DTOAccountInfo();
            Console.WriteLine("\nSilmek istediğiniz hesap numarasını girin: ");
            dTOAccount.HesapNo = long.Parse(Console.ReadLine());
            _ = eAccount.DeleteAccountInfo(dTOAccount);
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

                                dataAccessLayer.LogTransaction(transactionLog);

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

                                dataAccessLayer.LogTransaction(transactionLog);

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
                dataAccessLayer.LogError(ex, method.ToString());

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

                                    dataAccessLayer.LogTransaction(transactionLog);

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

                                    dataAccessLayer.LogTransaction(transactionLog);

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
                dataAccessLayer.LogError(ex, method.ToString());

                // Handle the error (display a user-friendly message, rollback transactions, etc.)
                Console.WriteLine("An error occurred while performing ParaCekme operation. Please try again later.");
            }
        }
    }
}

