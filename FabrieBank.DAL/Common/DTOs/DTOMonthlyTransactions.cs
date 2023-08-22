namespace FabrieBank.DAL.Common.DTOs
{
	public class DTOMonthlyTransactions
	{
		public DateTime FirstDayOfMonth { get; set; }
        public DateTime LastDayOfMonth { get; set; }
        public DateTime ThisDayOfMonth { get; set; }
    }
}