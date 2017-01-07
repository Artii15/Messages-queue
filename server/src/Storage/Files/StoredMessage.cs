using System;

namespace Server.Storage.Files
{
	[Serializable]
	public class StoredMessage
	{
		public string Content { get; set; }
		public string Next { get; set; }
	}
}
