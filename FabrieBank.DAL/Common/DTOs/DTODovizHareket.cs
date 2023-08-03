namespace FabrieBank.DAL.Common.DTOs
{
    public class DTODovizHareket
    {
        public long KaynakHesapNo { get; set; }
        public long HedefHesapNo { get; set; }
        public int KaynakDovizCinsi { get; set; }
        public int HedefDovizCinsi { get; set; }
        public decimal Miktar { get; set; }
    }
}