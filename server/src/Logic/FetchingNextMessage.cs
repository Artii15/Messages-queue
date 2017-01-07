using System;
using System.Collections.Concurrent;
using System.Threading;
using Server.Storage;

namespace Server.Logic
{
	public class FetchingNextMessage
	{
		ConcurrentDictionary<string, ManualResetEventSlim> WaitOnMessageEvents;
		MessagesStorage MessagesStorage;

		public FetchingNextMessage(ConcurrentDictionary<string, ManualResetEventSlim> waitOnMessageEvents,
		                           MessagesStorage messagesStorage)
		{
			WaitOnMessageEvents = waitOnMessageEvents;
			MessagesStorage = messagesStorage;
		}

		public string Fetch(string queueName)
		{
			ManualResetEventSlim waitOnMessageEvent;
			if (WaitOnMessageEvents.TryGetValue(queueName, out waitOnMessageEvent))
			{
				return readMessageFromStorage(queueName, waitOnMessageEvent);
			}
			throw new Exception(); //TODO More specific exception
		}

		string readMessageFromStorage(string queueName, ManualResetEventSlim waitOnMessageEvent)
		{
			string messageInQueue = null;
			while (messageInQueue == null)
			{
				waitOnMessageEvent.Wait();
				messageInQueue = MessagesStorage.TryToReadNextMessage(queueName);
			}

			return messageInQueue;
		}
	}
}