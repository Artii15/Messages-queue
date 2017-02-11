using System;
using System.Data;
using RestSharp;
using Server.Entities;
using Server.Services.Subscriptions.Create;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class CreatingSubscription
	{
		readonly Connections Connections;
		Locks Locks;

		public CreatingSubscription(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Create(CreateSubscription request)
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
					Create(connection, request);
				}
			}
		}

		void Create(IDbConnection connection, CreateSubscription request)
		{
			var subscriber = new Subscriber
			{
				CreationTime = request.CreationTime.HasValue ? request.CreationTime.Value : DateTime.UtcNow,
				LastAnnouncementId = null,
				Id = request.SubscriberId
			};

			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				request.CreationTime = subscriber.CreationTime;
				PropagateRequest(request);
			}

			connection.Insert(subscriber);
		}

		void PropagateRequest(CreateSubscription request)
		{
			var client = new RestClient(request.Cooperator);

			var requestToSend = new RestRequest($"topics/{request.TopicName}/subscriptions", Method.POST);
			requestToSend.AddParameter("CreationTime", request.CreationTime);
			requestToSend.AddParameter("SubscriberId", request.SubscriberId);

			client.Execute(requestToSend);
		}
	}
}
