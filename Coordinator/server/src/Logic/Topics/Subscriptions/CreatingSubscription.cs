using System.Data;
using RestSharp;

namespace Server
{
	public class CreatingSubscription : BasicTopicOperation
	{
		public CreatingSubscription(IDbConnection dbConnection) : base (dbConnection)
		{
		}

		public void Create(CreateSubscription request, int subscriberId)
		{
			var requestToSend = new RestRequest($"topics/{request.TopicName}/subscriptions", Method.POST);
			requestToSend.AddParameter("SubscriberId", subscriberId);

			processRequest(request.TopicName, requestToSend);
		}
	}
}
