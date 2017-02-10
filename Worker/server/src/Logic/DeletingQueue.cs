using System.Threading;
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
			lock(queueLock)
			{
				Connections.RemoveQueue(request.QueueName);
				Monitor.PulseAll(queueLock);
				Locks.RemoveQueueLock(request.QueueName);
			}
		}
	}
}
