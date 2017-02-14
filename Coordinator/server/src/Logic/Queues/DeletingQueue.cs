using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingQueue : BasicQueueOperation
	{

		public DeletingQueue(IDbConnection dbConnection) : base (dbConnection)
		{
		}

		public void Delete(DeleteQueue request)
		{ 
			var requestToSend = new RestRequest($"queues/{request.QueueName}", Method.DELETE);

			if (!QueuesQueries.QueueExists(DBConnection, request.QueueName))
				throw new QueueNotExistsException();
			else
			{
				var queue = QueuesQueries.getQueueByName(DBConnection, request.QueueName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, queue.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, queue.Cooperator);

				QueuesQueries.DeleteQueue(DBConnection, request.QueueName);
				PropageteRequestToWorkers(requestToSend, queue, worker, coworker);
			}
		}

	}
}
