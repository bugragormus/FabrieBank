namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOTransfer
    {
        public int SourceAccountIndex { get; set; }
        public int TargetAccountIndex { get; set; }
        public long SourceAccountNo { get; set; }
        public long TargetAccountNo { get; set; }
        public decimal Amount { get; set; }
    }
}