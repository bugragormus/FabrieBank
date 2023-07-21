using FabrieBank.Common;
using FabrieBank.Common.Enums;
using FabrieBank.Entity;

namespace FabrieBank.BLL
{
	public class AccountLogic
	{
        public string GetDovizCinsi(EnumDovizCinsleri.DovizCinsleri dovizCins)
        {
            switch (dovizCins)
            {
                case EnumDovizCinsleri.DovizCinsleri.TL:
                    return "Türk Lirası (TL)";
                case EnumDovizCinsleri.DovizCinsleri.USD:
                    return "Amerikan Doları (USD)";
                case EnumDovizCinsleri.DovizCinsleri.EUR:
                    return "Euro (EUR)";
                case EnumDovizCinsleri.DovizCinsleri.GAU:
                    return "Gram Altın (GAU)";
                case EnumDovizCinsleri.DovizCinsleri.XAG:
                    return "Gram Gümüş (XAG)";
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
    }
}

