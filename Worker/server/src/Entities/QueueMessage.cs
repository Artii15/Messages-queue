namespace Server.Entities
{
	public class QueueMessage
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public int Author { get; set; }
		public bool Readed { get; set; }
	}
}
