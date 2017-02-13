using System.Data;
using RestSharp;
using System;

namespace Server
{
	public class CreatingQueue
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public CreatingQueue(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Create(CreateQueue request)
		{
			if (QueuesQueries.QueueExists(DBConnection, request.Name))
				throw new QueueAlreadyExistsException();
			else
			{
				Worker worker, coworker;
				CalculateQueueWorkers(request.Name, out worker, out coworker);
				QueuesQueries.CreateQueue(DBConnection, request.Name, worker.Id, coworker.Id);

				if (WorkerQueries.IsWorkerAlive(DBConnection, worker.Id))
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

		void CalculateQueueWorkers(string queueName, out Worker worker, out Worker coworker)
		{
			var workerCount = WorkerQueries.GetWorkersCount(DBConnection);
			var nameHash = Math.Abs(queueName.GetHashCode());
			var workerPosition = nameHash % workerCount;
			var coworkerPosition = (nameHash + 1) % workerCount;
			worker = WorkerQueries.GetWorker(DBConnection, workerPosition);
			coworker = WorkerQueries.GetWorker(DBConnection, coworkerPosition);
		}

		IRestResponse PropagateRequest(CreateQueue request, Worker worker, Worker coworker)
		{
			var client = new RestClient(worker.Address);
			client.Timeout = TIMEOUT;
			var requestToSend = new RestRequest("queues", Method.POST);
			requestToSend.AddParameter("Name", request.Name);
			requestToSend.AddParameter("Cooperator", coworker.Address);
			return client.Execute(requestToSend);
		}

		void PropagateRequestToCoworker(CreateQueue request, Worker coworker)
		{
			var coworkerClient = new RestClient(coworker.Address);
			var coworkerRequestToSend = new RestRequest("queues", Method.POST);
			coworkerRequestToSend.AddParameter("Name", request.Name);
			coworkerClient.Execute(coworkerRequestToSend);
		}
	}
}
