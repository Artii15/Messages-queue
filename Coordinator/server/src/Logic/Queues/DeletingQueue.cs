using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingQueue
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public DeletingQueue(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Delete(DeleteQueue request)
		{ 
			if (!QueuesQueries.QueueExists(DBConnection, request.QueueName))
				throw new QueueNotExistsException();
			else
			{
				var queue = QueuesQueries.getQueueByName(DBConnection, request.QueueName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, queue.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, queue.Cooperator);

				QueuesQueries.DeleteQueue(DBConnection, request.QueueName);
				if (worker.Alive)
				{
					var response = PropagateRequest(request, worker, coworker);
					if (response.ResponseStatus == ResponseStatus.TimedOut ||
						response.ResponseStatus == ResponseStatus.Error)
						PropagateRequestToCoworker(request, coworker);
				}
				else
					PropagateRequestToCoworker(request, coworker);
			}
		}

		IRestResponse PropagateRequest(DeleteQueue request, Worker worker, Worker coworker)
		{
			var client = new RestClient($"http://{worker.Address}");
			client.Timeout = TIMEOUT;
			var requestToSend = new RestRequest($"queues/{request.QueueName}", Method.DELETE);
			if (coworker.Alive)
				requestToSend.AddParameter("Cooperator", coworker.Address);
			return client.Execute(requestToSend);
		}

		void PropagateRequestToCoworker(DeleteQueue request, Worker coworker)
		{
			var coworkerClient = new RestClient($"http://{coworker.Address}");
			var coworkerRequestToSend = new RestRequest($"queues/{request.QueueName}", Method.DELETE);
			coworkerClient.Execute(coworkerRequestToSend);
		}
	}
}
