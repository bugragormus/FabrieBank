namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOCustomer
    {
        public int CustomerId { get; set; }
        public string? Name { get; set; }
        public string? Lastname { get; set; }
        public long Tckn { get; set; }
        public int Password { get; set; }
        public long CellNo { get; set; }
        public string? Email { get; set; }
    }
}