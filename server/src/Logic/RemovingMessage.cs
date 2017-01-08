using Server.Storage;

namespace Server.Logic
{
	public class RemovingMessage
	{
		readonly MessagesStorage MessagesStorage;

		public RemovingMessage(MessagesStorage messagesStorage)
		{
			MessagesStorage = messagesStorage;
		}

		public MessageRemovingStatus TryToPop(string queueName, string messageId)
		{
			return MessagesStorage.TryToPop(queueName, messageId);
		}
	}
}
