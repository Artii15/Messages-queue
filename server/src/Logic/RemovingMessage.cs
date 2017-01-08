using System.Collections.Concurrent;
using System.Threading;
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
			var removingStatus = MessagesStorage.TryToPop(queueName, messageId);
			if (removingStatus.NextMessageId == "")
			{
				
			}

			return removingStatus;
		}
	}
}
