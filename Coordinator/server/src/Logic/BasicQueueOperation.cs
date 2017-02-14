using System.Data;
using System.Net;
using RestSharp;

namespace Server
{
	public abstract class BasicQueueOperation : BasicOperation
	{
		public BasicQueueOperation(IDbConnection dbConnection) : base(dbConnection)
		{
		}

		protected void processRequest(string queueName, RestRequest request)
		{
			if (!QueuesQueries.QueueExists(DBConnection, queueName))
				throw new QueueNotExistsException();
			else
			{
				var queue = QueuesQueries.getQueueByName(DBConnection, queueName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, queue.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, queue.Cooperator);

				PropageteRequestToWorkers(request, queue, worker, coworker);
			}
		}

		protected void PropageteRequestToWorkers(RestRequest request, Queue queue, Worker worker, Worker coworker)
		{
			var requestForMaster = request.AddParameter("Cooperator", coworker.Address);
			IRestResponse response;

			if (WorkerQueries.IsWorkerAlive(DBConnection, worker.Id))
			{
				response = PropagateRequest(requestForMaster, worker);
				if (response.ResponseStatus == ResponseStatus.TimedOut ||
					response.ResponseStatus == ResponseStatus.Error)
				{
					response = PropagateRequest(request, coworker);
					QueuesQueries.swapWorkers(DBConnection, queue);
				}
			}
			else
			{
				response = PropagateRequest(request, coworker);
				QueuesQueries.swapWorkers(DBConnection, queue);
			}
			if (response.StatusCode != HttpStatusCode.OK)
				throw new BadRequestException();
		}

		IRestResponse PropagateRequest(IRestRequest request, Worker worker)
		{
			var client = new RestClient(worker.Address);
			client.Timeout = TIMEOUT;
			return client.Execute(request);
		}
	}
}
