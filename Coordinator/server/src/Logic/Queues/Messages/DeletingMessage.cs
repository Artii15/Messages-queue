using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingMessage : BasicQueueOperation
	{
		public DeletingMessage(IDbConnection dbConnection) : base (dbConnection)
		{
		}

		public void Delete(DeleteMessage request)
		{
			var requestToSend = new RestRequest($"/queues/{request.QueueName}/messages/{request.MessageId}", Method.DELETE);

			processRequest(request.QueueName, requestToSend);
		}
	}
}
