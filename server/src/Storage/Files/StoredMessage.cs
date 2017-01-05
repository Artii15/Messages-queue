using Server.Entities;

namespace Server.Storage.Files
{
	public class StoredMessage
	{
		public Message message { get; set; }
		public string next { get; set; }
	}
}
