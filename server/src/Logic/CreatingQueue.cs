using System.Collections.Concurrent;
using System.Threading;
using Server.Storage;

namespace Server.Logic
{
	public class CreatingQueue
	{
		readonly QueuesStorage QueuesStorage;
		readonly ConcurrentDictionary<string, ManualResetEventSlim> MessagesEvents;

		public CreatingQueue(QueuesStorage queuesStorage,
		                    ConcurrentDictionary<string, ManualResetEventSlim> messagesEvents)
		{
			QueuesStorage = queuesStorage;
			MessagesEvents = messagesEvents;
		}

		public void Create(string queueName)
		{
			if (MessagesEvents.TryAdd(queueName, new ManualResetEventSlim(false)))
			{
				QueuesStorage.Create(queueName);
			}
			else
			{
				//TODO Throw already exists exception
			}
		}
	}
}
