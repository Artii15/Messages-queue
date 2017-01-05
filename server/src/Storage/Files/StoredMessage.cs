using Server.Entities;

namespace Server.Storage.Files
{
	public class StoredMessage
	{
		public Message Message { get; set; }
		public string Next { get; set; }
	}
}
