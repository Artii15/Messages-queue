using Server.Storage;

namespace Server.Logic
{
	public class CreatingMessage
	{
		MessagesStorage MessagesStorage;

		public CreatingMessage(MessagesStorage messagesStorage)
		{
			MessagesStorage = messagesStorage;
		}

		public void Create(string queueName, string message)
		{
			MessagesStorage.Create(queueName, message);
		}
	}
}
