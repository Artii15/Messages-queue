using System;
using System.Collections.Concurrent;
using System.Threading;
using Server.Storage;

namespace Server.Logic
{
	public class CreatingMessage
	{
		MessagesStorage MessagesStorage;
		ConcurrentDictionary<string, ManualResetEventSlim> MessagesEvents;

		public CreatingMessage(MessagesStorage messagesStorage,
		                       ConcurrentDictionary<string, ManualResetEventSlim> messagesEvents)
		{
			MessagesStorage = messagesStorage;
			MessagesEvents = messagesEvents;
		}

		public void Create(string queueName, string message)
		{
			ManualResetEventSlim messageEvent;
			if (MessagesEvents.TryGetValue(queueName, out messageEvent))
			{
				MessagesStorage.Create(queueName, message);
				messageEvent.Set();
			}
			else
			{
				throw new Exception(); //TODO more specific exception
			}
		}
	}
}
