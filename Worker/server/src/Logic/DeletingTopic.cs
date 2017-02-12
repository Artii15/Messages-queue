using System;
using System.Threading;
using RestSharp;
using Server.Services.Topics.Delete;

namespace Server.Logic
{
	public class DeletingTopic
	{
		Connections Connections;
		Locks Locks;

		public DeletingTopic(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Delete(DeleteTopic request)
		{
			var topicLock = Locks.TakeTopicLock(request.TopicName);
			lock (topicLock)
			{
				if (Locks.TopicsRecoveryLocks.ContainsKey(request.TopicName))
				{
					throw new Exception($"Topic {request.TopicName} is inconsistent");
				}

				Propagate(request);
				Connections.RemoveTopic(request.TopicName);
				Monitor.PulseAll(topicLock);
				Locks.RemoveTopicLock(request.TopicName);
			}
		}

		void Propagate(DeleteTopic request)
		{
			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				var client = new RestClient(request.Cooperator);

				var requestToSend = new RestRequest($"topics/{request.TopicName}", Method.DELETE);

				client.Execute(requestToSend);
			}
		}
	}
}
