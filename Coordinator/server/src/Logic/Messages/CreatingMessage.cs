using System.Data;
using RestSharp;
namespace Server
{
	public class CreatingMessage
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public CreatingMessage(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Create(CreateMessage request)
		{ 
			if (!QueuesQueries.QueueExists(DBConnection, request.QueueName))
				throw new QueueNotExistsException();
			else
			{
				var queue = QueuesQueries.getQueueByName(DBConnection, request.QueueName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, queue.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, queue.Cooperator);

				var client = new RestClient("http://" + worker.Address);
				client.Timeout = TIMEOUT;
				var requestToSend = new RestRequest($"queues/{request.QueueName}/messages", Method.POST);
				requestToSend.AddParameter("Content", request.Content);
				requestToSend.AddParameter("Cooperator", coworker.Address);
				var response = client.Execute(requestToSend);
				if (response.ResponseStatus == ResponseStatus.TimedOut ||
					response.ResponseStatus == ResponseStatus.Error)
				{
					var coworkerClient = new RestClient("http://" + coworker.Address);
					var coworkerRequestToSend = new RestRequest($"queues/{request.QueueName}/messages", Method.POST);
					coworkerRequestToSend.AddParameter("Content", request.Content);
					coworkerClient.Execute(requestToSend);
				}
			}
		}
	}
}
