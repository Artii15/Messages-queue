using System;
using System.Data;
using System.Net;
using RestSharp;

namespace Server
{
	public class GettingMessage : BasicGettingOperation
	{
		public GettingMessage(IDbConnection dbConnection) : base (dbConnection)
		{
		}

		public Message Get(GetMessage request)
		{
			var requestToSend = new RestRequest($"queues/{request.QueueName}/messages", Method.GET);

			if (!QueuesQueries.QueueExists(DBConnection, request.QueueName))
				throw new QueueNotExistsException();
			else
			{
				var queue = QueuesQueries.getQueueByName(DBConnection, request.QueueName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, queue.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, queue.Cooperator);

				return PropageteRequestToWorkers<Message>(requestToSend, queue, worker, coworker);
			}
		}

		protected override void swapWorkers(ICollection collection)
		{
			QueuesQueries.swapWorkers(DBConnection, (Queue)collection);
		}
	}
}
