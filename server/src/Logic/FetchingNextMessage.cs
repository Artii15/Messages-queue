using System;
using System.Collections.Concurrent;
using System.Threading;
using Server.Entities;
using Server.Storage;

namespace Server.Logic
{
	public class FetchingNextMessage
	{
		readonly MessagesStorage MessagesStorage;
		ConcurrentDictionary<string, ManualResetEventSlim> WaitOnMessageEvents;

		public FetchingNextMessage(MessagesStorage messagesStorage,
		                          ConcurrentDictionary<string, ManualResetEventSlim> waitOnMessageEvents)
		{
			MessagesStorage = messagesStorage;
			WaitOnMessageEvents = waitOnMessageEvents;
		}

		public Message Fetch(string queueName)
		{
			ManualResetEventSlim waitOnMessageEvent;
			if (WaitOnMessageEvents.TryGetValue(queueName, out waitOnMessageEvent))
			{
				return readMessageFromStorage(queueName, waitOnMessageEvent);
			}
			throw new Exception(); //TODO More specific exception
		}

		Message readMessageFromStorage(string queueName, ManualResetEventSlim waitOnMessageEvent)
		{
			Message? messageInQueue = null;
			while (messageInQueue == null)
			{
				waitOnMessageEvent.Wait();
				messageInQueue = MessagesStorage.TryToReadNextMessage(queueName);
			}
			return (Message)messageInQueue;
		}
	}
}