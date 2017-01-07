using System.Collections.Generic;
using System.Threading;
using Server.Storage;

namespace Server.Logic
{
	public class FetchingNextMessage
	{
		Dictionary<string, ManualResetEventSlim> WaitOnMessageEvents;
		MessagesStorage MessagesStorage;

		public FetchingNextMessage(Dictionary<string, ManualResetEventSlim> waitOnMessageEvents,
		                           MessagesStorage messagesStorage)
		{
			WaitOnMessageEvents = waitOnMessageEvents;
			MessagesStorage = messagesStorage;
		}

		public string Fetch(string queueName)
		{
			var waitOnMessageEvent = WaitOnMessageEvents[queueName];

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