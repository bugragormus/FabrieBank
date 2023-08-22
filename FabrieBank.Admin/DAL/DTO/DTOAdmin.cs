namespace FabrieBank.Admin.DAL.DTO
{
	public class DTOAdmin
	{
		public int Id { get; set; }
		public string? Nickname { get; set; }
        public string? Password { get; set; }
        public int AccessLevel { get; set; }
    }
}