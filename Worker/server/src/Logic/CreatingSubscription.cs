using RestSharp;
using Server.Entities;
using Server.Services.Subscriptions.Create;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class CreatingSubscription
	{
		readonly Connections Connections;
		readonly Locks Locks;

		public CreatingSubscription(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Create(CreateSubscription request)
		{
			var connection = Connections.ConnectToInitializedTopic(request.TopicName);

			var subscriber = new Subscriber { 
				CreationTime = request.CreationTime.HasValue ? request.CreationTime.Value : System.DateTime.UtcNow, 
				LastAnnouncementId = null,
				Id = request.SubscriberId
			};
			connection.Insert(subscriber);

			connection.Close();

			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				request.CreationTime = subscriber.CreationTime;
				PropagateRequest(request);
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
