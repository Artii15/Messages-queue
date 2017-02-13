using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingMessage
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public DeletingMessage(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Delete(DeleteMessage request)
		{
			if (!QueuesQueries.QueueExists(DBConnection, request.QueueName))
				throw new QueueNotExistsException();
			else
			{
				var queue = QueuesQueries.getQueueByName(DBConnection, request.QueueName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, queue.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, queue.Cooperator);

				if (WorkerQueries.IsWorkerAlive(DBConnection, worker.Id))
				{
					var response = PropagateRequest(request, worker, coworker);
					if (response.ResponseStatus == ResponseStatus.TimedOut ||
						response.ResponseStatus == ResponseStatus.Error)
					{
						PropagateRequestToCoworker(request, coworker);
						QueuesQueries.swapWorkers(DBConnection, queue);
					}
				}
				else
				{
					PropagateRequestToCoworker(request, coworker);
					QueuesQueries.swapWorkers(DBConnection, queue);
				}
			}
		}

		IRestResponse PropagateRequest(DeleteMessage request, Worker worker, Worker coworker)
		{
			var client = new RestClient(worker.Address);
			client.Timeout = TIMEOUT;
			var requestToSend = new RestRequest($"/queues/{request.QueueName}/messages/{request.MessageId}", Method.DELETE);
			requestToSend.AddParameter("Cooperator", coworker.Address);
			return client.Execute(requestToSend);
		}

		void PropagateRequestToCoworker(DeleteMessage request, Worker coworker)
		{
			var coworkerClient = new RestClient(coworker.Address);
			var coworkerRequestToSend = new RestRequest($"/queues/{request.QueueName}/messages/{request.MessageId}", Method.DELETE);
			coworkerClient.Execute(coworkerRequestToSend);
		}
	}
}
