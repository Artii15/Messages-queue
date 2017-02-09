using ServiceStack.DataAnnotations;

namespace Server
{
	public class Worker
	{
		[AutoIncrement]
		public long Id { get; set; }
		public string Address { get; set; }
		public bool Alive { get; set; }
	}
}
