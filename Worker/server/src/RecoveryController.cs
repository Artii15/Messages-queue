using System.Collections.Generic;
using RestSharp;
using Server.Entities;

namespace Server
{
	public class RecoveryController
	{
		Dictionary<string, RestClient> Workers;
		QueuesAndTopics QueuesAndTopicsToRecover;

		public void BeginRecovery(QueuesAndTopics queuesAndTopicsToRecover)
		{
			QueuesAndTopicsToRecover = queuesAndTopicsToRecover;
			Workers = new Dictionary<string, RestClient>();

		}

		void RecoverQueues()
		{
			foreach (var queue in QueuesAndTopicsToRecover.Queues)
			{
				RecoverMessagesContainer(queue.Value, "queues");
			}
		}

		void RecoverTopics()
		{
			foreach (var topic in QueuesAndTopicsToRecover.Topics)
			{
				RecoverMessagesContainer(topic.Value, "topics");
			}
		}

		void RecoverMessagesContainer(MessagesContainer container, string address)
		{
			var workerAddress = container.GetCooperator();
			var worker = Workers.ContainsKey(workerAddress) ? Workers[workerAddress] : new RestClient(workerAddress);
			Workers[workerAddress] = worker;

			var request = new RestRequest($"failures/{address}", Method.POST);
			request.AddBody(new { Name = container.GetName() });
			worker.Execute(request);
		}
	}
}
