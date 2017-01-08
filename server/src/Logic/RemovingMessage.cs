using System.Collections.Concurrent;
using System.Threading;
using Server.Storage;

namespace Server.Logic
{
	public class RemovingMessage
	{
		readonly MessagesStorage MessagesStorage;
		readonly ConcurrentDictionary<string, ManualResetEventSlim> MessagesEvents;

		public RemovingMessage(MessagesStorage messagesStorage,
		                      ConcurrentDictionary<string, ManualResetEventSlim> messagesEvents)
		{
			MessagesStorage = messagesStorage;
			MessagesEvents = messagesEvents;
		}

		public MessageRemovingStatus TryToPop(string queueName, string messageId)
		{
			var removingStatus = MessagesStorage.TryToPop(queueName, messageId);
			if (removingStatus.NextMessageId == "")
			{
				MessagesEvents[queueName].Reset();
			}

			return removingStatus;
		}
	}
}
