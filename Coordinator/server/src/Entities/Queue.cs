using ServiceStack.DataAnnotations;

namespace Server
{
	public class Queue : ICollection
	{
		[AutoIncrement]
		public long Id { get; set; }
		[Index(Unique = true)]
		public string Name { get; set; }
		[References(typeof(Worker))]
		public long Worker { get; set; }
		[References(typeof(Worker))]
		public long Cooperator { get; set; }
	}
}
