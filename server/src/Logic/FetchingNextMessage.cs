using System.Threading;
using Server.Entities;
using Server.Storage;

namespace Server.Logic
{
	public class FetchingNextMessage
	{
		readonly MessagesStorage MessagesStorage;

		public FetchingNextMessage(MessagesStorage messagesStorage)
		{
			MessagesStorage = messagesStorage;
		}

		public Message Fetch(string queueName)
		{
			return MessagesStorage.ReadNextMessage(queueName);
		}

		Message readMessageFromStorage(string queueName, ManualResetEventSlim waitOnMessageEvent)
		{
			Message? messageInQueue = null;
			while (messageInQueue == null)
			{
				waitOnMessageEvent.Wait();
				messageInQueue = MessagesStorage.ReadNextMessage(queueName);
			}
			return (Message)messageInQueue;
		}
	}
}