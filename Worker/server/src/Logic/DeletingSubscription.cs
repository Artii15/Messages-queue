using System;
using RestSharp;
using Server.Entities;
using Server.Services.Subscriptions.Delete;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class DeletingSubscription
	{
		readonly Connections Connections;
		Locks Locks;

		public DeletingSubscription(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Delete(DeleteSubscription request)
		{
			var topicLock = Locks.TakeTopicLock(request.TopicName);
			lock (topicLock)
			{
				if (Locks.TopicsRecoveryLocks.ContainsKey(request.TopicName))
				{
					throw new Exception($"Topic {request.TopicName} is inconsistent");
				}

				using (var connection = Connections.ConnectToInitializedTopic(request.TopicName))
				{
					Propagate(request);
					connection.DeleteById<Subscriber>(request.SubscriberId);
				}
			}

		}

		void Propagate(DeleteSubscription request)
		{
			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				var client = new RestClient(request.Cooperator);
				var requestToSend = new RestRequest($"topics/{request.TopicName}/subscriptions/{request.SubscriberId}", Method.DELETE);
				client.Execute(requestToSend);
			}
		}
	}
}