using System.Data;
using RestSharp;

namespace Server
{
	public class GettingMessage
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public GettingMessage(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public Message Get(GetMessage request)
		{
			if (!QueuesQueries.QueueExists(DBConnection, request.QueueName))
				throw new QueueNotExistsException();
			else
			{
				var queue = QueuesQueries.getQueueByName(DBConnection, request.QueueName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, queue.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, queue.Cooperator);
				IRestResponse<Message> response;

				if (worker.Alive)
				{
					response = PropagateRequest(request, worker, coworker);
					if (response.ResponseStatus == ResponseStatus.TimedOut ||
						response.ResponseStatus == ResponseStatus.Error)
						response = PropagateRequestToCoworker(request, coworker);
				}
				else
					response = PropagateRequestToCoworker(request, coworker);

				return response.Data;
			}
		}

		IRestResponse<Message> PropagateRequest(GetMessage request, Worker worker, Worker coworker)
		{
			var client = new RestClient($"http://{worker.Address}");
			client.Timeout = TIMEOUT;
			var requestToSend = new RestRequest($"queues/{request.QueueName}/messages", Method.GET);
			return client.Execute<Message>(requestToSend);
		}

		IRestResponse<Message> PropagateRequestToCoworker(GetMessage request, Worker coworker)
		{
			var coworkerClient = new RestClient($"http://{coworker.Address}");
			var coworkerRequestToSend = new RestRequest($"queues/{request.QueueName}/messages", Method.GET);
			return coworkerClient.Execute<Message>(coworkerRequestToSend);
		}
	}
}
