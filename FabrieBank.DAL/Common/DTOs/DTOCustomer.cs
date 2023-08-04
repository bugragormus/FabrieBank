namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOCustomer
    {
        public int MusteriId { get; set; }
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public long Tckn { get; set; }
        public int Sifre { get; set; }
        public long TelNo { get; set; }
        public string? Email { get; set; }
    }
}