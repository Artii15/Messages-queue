using System.Data;
using RestSharp;
using System;

namespace Server
{
	public class CreatingQueue : BasicQueueOperation
	{

		public CreatingQueue(IDbConnection dbConnection) : base(dbConnection)
		{
		}

		public void Create(CreateQueue request)
		{
			var requestToSend = new RestRequest("queues", Method.POST);
			requestToSend.AddParameter("Name", request.Name);

			if (QueuesQueries.QueueExists(DBConnection, request.Name))
				throw new QueueAlreadyExistsException();
			else
			{
				Worker worker, coworker;
				CalculateQueueWorkers(request.Name, out worker, out coworker);
				QueuesQueries.CreateQueue(DBConnection, request.Name, worker.Id, coworker.Id);
				var queue = QueuesQueries.getQueueByName(DBConnection, request.Name);
				PropageteRequestToWorkers(requestToSend, queue, worker, coworker);
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
	}
}
