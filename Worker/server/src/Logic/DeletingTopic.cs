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
		Propagators Propagators;

		public DeletingTopic(Connections connections, Locks locks, Propagators propagators)
		{
			Connections = connections;
			Locks = locks;
			Propagators = propagators;
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

				Connections.RemoveTopic(request.TopicName);
				Monitor.PulseAll(topicLock);
				Propagators.ScheduleTopicOperation(request.TopicName, () => Propagate(request));
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
