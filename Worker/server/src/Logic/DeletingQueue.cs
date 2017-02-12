using System;
using System.Threading;
using RestSharp;
using Server.Services.Queues.Delete;

namespace Server.Logic
{
	public class DeletingQueue
	{
		readonly Connections Connections;
		readonly Locks Locks;

		public DeletingQueue(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Delete(DeleteQueue request)
		{
			var queueLock = Locks.TakeQueueLock(request.QueueName);
			lock (queueLock)
			{
				if (Locks.QueuesRecoveryLocks.ContainsKey(request.QueueName))
				{
					throw new Exception($"Queue {request.QueueName} is inconsistent");
				}

				Propagate(request);
				Connections.RemoveQueue(request.QueueName);
				Monitor.PulseAll(queueLock);
				Locks.RemoveQueueLock(request.QueueName);
			}
		}

		void Propagate(DeleteQueue request)
		{
			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				var client = new RestClient(request.Cooperator);

				var requestToSend = new RestRequest($"queues/{request.QueueName}", Method.DELETE);

				client.Execute(requestToSend);
			}
		}
	}
}
