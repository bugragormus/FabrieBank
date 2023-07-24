using FabrieBank.Common.Enums;
namespace FabrieBank.Common
{
    public class DTOAccountInfo
    {
        public long HesapNo { get; set; }
        public decimal Bakiye { get; set; }
        public int MusteriId { get; set; }
        public EnumDovizCinsleri.DovizCinsleri DovizCins { get; set; }
        public string HesapAdi { get; set; }
    }
}