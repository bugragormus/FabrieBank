using FabrieBank.Common.Enums;

namespace FabrieBank
{
    public static class TransactionFees
    {
        public static long GetTransactionFee(EnumDovizCinsleri.DovizCinsleri dovizCinsi)
        {
            switch (dovizCinsi)
            {
                case EnumDovizCinsleri.DovizCinsleri.TL:
                    return 0;
                case EnumDovizCinsleri.DovizCinsleri.USD:
                    return 1;
                case EnumDovizCinsleri.DovizCinsleri.EUR:
                    return 2;
                case EnumDovizCinsleri.DovizCinsleri.GAU:
                    return 3;
                case EnumDovizCinsleri.DovizCinsleri.XAG:
                    return 4;
                default:
                    return 0;
            }
        }
    }
}
