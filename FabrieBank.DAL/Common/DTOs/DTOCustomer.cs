namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOCustomer
    {
        public int CustomerId { get; set; }
        public string? Name { get; set; }
        public string? Lastname { get; set; }
        public long Tckn { get; set; }
        public string? Password { get; set; }
        public long CellNo { get; set; }
        public string? Email { get; set; }
        public int Status { get; set; }
    }
}