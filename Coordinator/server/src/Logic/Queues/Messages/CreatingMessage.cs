using System.Data;
using RestSharp;
namespace Server
{
	public class CreatingMessage : BasicQueueOperation
	{
		public CreatingMessage(IDbConnection dbConnection) : base(dbConnection)
		{
		}

		public void Create(CreateMessage request)
		{ 
			var requestToSend = new RestRequest($"queues/{request.QueueName}/messages", Method.POST);
			requestToSend.AddParameter("Content", request.Content);

			processRequest(request.QueueName, requestToSend);
		}
	}
}
