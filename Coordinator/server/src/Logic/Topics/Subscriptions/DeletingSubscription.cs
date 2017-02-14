using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingSubscription : BasicTopicOperation
	{
		public DeletingSubscription(IDbConnection dbConnection) : base (dbConnection)
		{
		}

		public void Delete(DeleteSubscription request, int subscriberId)
		{ 
			var requestToSend = new RestRequest($"topics/{request.TopicName}/subscriptions/{subscriberId}", Method.DELETE);

			processRequest(request.TopicName, requestToSend);
		}
	}
}
