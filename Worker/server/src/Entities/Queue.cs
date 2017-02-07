using ServiceStack.DataAnnotations;

namespace Server.Entities
{
	public class Queue
	{
		[AutoIncrement]
		public int Id { get; set; }

		[Index(Unique=true)]
		public string Name { get; set; }

		[References(typeof(Cooperator))]
		public int CooperatorId { get; set; }
	}
}
