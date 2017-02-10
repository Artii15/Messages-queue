using RestSharp;
using Server.Entities;
using Server.Services.Subscriptions.Delete;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class DeletingSubscription
	{
		readonly Connections Connections;

		public DeletingSubscription(Connections connections)
		{
			Connections = connections;
		}

		public void Delete(DeleteSubscription request)
		{
			Propagate(request);
			using (var connection = Connections.ConnectToInitializedTopic(request.TopicName))
			{
				connection.DeleteById<Subscriber>(request.SubscriberId);
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