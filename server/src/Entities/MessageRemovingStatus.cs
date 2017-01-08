namespace Server
{
	public class MessageRemovingStatus
	{
		public bool WasMessageRemoved { get; }
		public string NextMessageId { get; }

		public MessageRemovingStatus(bool wasMessageRemoved, string nextMessageId)
		{
			WasMessageRemoved = wasMessageRemoved;
			NextMessageId = nextMessageId;
		}
	}
}
