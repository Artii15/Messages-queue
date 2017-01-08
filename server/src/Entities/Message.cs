namespace Server.Entities
{
	public struct Message
	{
		public string Id { get; }
		public string Content { get; }

		public Message(string id, string content)
		{
			Id = id;
			Content = content;
		}
	}
}
