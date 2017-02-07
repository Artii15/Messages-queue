using ServiceStack.DataAnnotations;

namespace Server.Entities
{
	public class Cooperator
	{
		[AutoIncrement]
		public int Id { get; set; }

		[Index(Unique = true)]
		public string Address { get; set; }
	}
}
