using System.Collections.Concurrent;
using System.Threading;
using RestSharp;
using Server.Entities;
using Server.Logic;

namespace Server
{
	public class RecoveryController
	{
		QueuesAndTopics QueuesAndTopicsToRecover;
		Locks Locks;
		readonly string ServerAddress;

		public RecoveryController(string serverAddress, Locks locks)
		{
			ServerAddress = serverAddress;
			Locks = locks;
		}

		public void BeginRecovery(QueuesAndTopics queuesAndTopicsToRecover)
		{
			QueuesAndTopicsToRecover = queuesAndTopicsToRecover;
			RecoverQueues();
			RecoverTopics();
		}

		void RecoverQueues()
		{
			foreach (var queue in QueuesAndTopicsToRecover.Queues)
			{
				var queueToRecover = queue.Value;
				var recoveryResponse = RecoverMessagesContainer(queueToRecover, "queues");
				HandleResponse(recoveryResponse, Locks.QueuesRecoveryLocks, queueToRecover.Name);
			}
		}

		void RecoverTopics()
		{
			foreach (var topic in QueuesAndTopicsToRecover.Topics)
			{
				var topicToRecover = topic.Value;
				var recoveryResponse = RecoverMessagesContainer(topicToRecover, "topics");
				HandleResponse(recoveryResponse, Locks.TopicsRecoveryLocks, topicToRecover.Name);
			}
		}

		void HandleResponse(IRestResponse response, ConcurrentDictionary<string, ManualResetEventSlim> recoveryLocks, string containerName)
		{
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				ManualResetEventSlim recoveryLock;
				if (recoveryLocks.TryRemove(containerName, out recoveryLock))
				{
					recoveryLock.Set();
				}
			}
		}

		IRestResponse RecoverMessagesContainer(MessagesContainer container, string category)
		{
			var worker = new RestClient(container.GetCooperator());
			var request = new RestRequest($"failures/{category}", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddJsonBody(new { Name = container.GetName(), Cooperator = ServerAddress });

			return worker.Execute(request);
		}
	}
}