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
		Propagators Propagators;

		public CreatingSubscription(Connections connections, Locks locks, Propagators propagators)
		{
			Connections = connections;
			Locks = locks;
			Propagators = propagators;
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
				CreationTime = request.CreationTime.HasValue ? DateTime.FromBinary(request.CreationTime.Value) : DateTime.UtcNow,
				LastAnnouncementId = null,
				Id = request.SubscriberId
			};
			connection.Insert(subscriber);

			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				request.CreationTime = subscriber.CreationTime.ToBinary();
				Propagators.ScheduleTopicOperation(request.TopicName, () => PropagateRequest(request));
			}
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