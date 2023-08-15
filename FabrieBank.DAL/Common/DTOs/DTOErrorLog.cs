namespace FabrieBank.DAL.Common.DTOs
{
	public class DTOErrorLog
	{
		public int ErrorId { get; set; }
		public DateTime ErrorDateTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? OperationName { get; set; }
    }
}

