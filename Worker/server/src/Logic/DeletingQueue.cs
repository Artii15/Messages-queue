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
		Propagators Propagators;

		public DeletingQueue(Connections connections, Locks locks, Propagators propagators)
		{
			Connections = connections;
			Locks = locks;
			Propagators = propagators;
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

				Connections.RemoveQueue(request.QueueName);
				Monitor.PulseAll(queueLock);
				Propagators.ScheduleQueueOperation(request.QueueName, () => Propagate(request));
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
