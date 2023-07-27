using FabrieBank.Common.Enums;

namespace FabrieBank.DTO
{
    public class DTODovizHareket
    {
        public long KaynakHesapNo { get; set; }
        public long HedefHesapNo { get; set; }
        public int DovizCinsi { get; set; }
        public decimal Miktar { get; set; }
    }
}