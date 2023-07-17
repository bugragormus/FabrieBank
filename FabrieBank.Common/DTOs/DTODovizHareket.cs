using FabrieBank.Common.Enums;

namespace FabrieBank.DTO
{
    public class DTODovizHareket
    {
        public long KaynakHesapNo { get; set; }
        public long HedefHesapNo { get; set; }
        public EnumDovizCinsleri.DovizCinsleri DovizCinsi { get; set; }
        public long Miktar { get; set; }
    }
}