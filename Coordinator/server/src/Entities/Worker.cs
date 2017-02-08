using ServiceStack.DataAnnotations;

namespace Server
{
	public class Worker
	{
		[AutoIncrement]
		public int Id { get; set; }
		public string Address { get; set; }
		public bool Alive { get; set; }
	}
}
