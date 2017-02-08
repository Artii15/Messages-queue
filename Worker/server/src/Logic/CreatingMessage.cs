using Server.Entities;
using Server.Services.Messages.Create;
using ServiceStack.OrmLite;
using System.Threading;

namespace Server.Logic
{
	public class CreatingMessage
	{
		readonly Connections Connections;
		readonly Locks Locks;

		public CreatingMessage(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Create(CreateMessage request)
		{
			var connection = Connections.ConnectToInitializedQueue(request.QueueName);
			var queueLock = Locks.TakeQueueLock(request.QueueName);

			Monitor.Enter(queueLock);

			connection.Insert(new QueueMessage
			{
				Author = request.Author,
				Content = request.Content,
				Readed = false
			});

			Monitor.PulseAll(queueLock);
			Monitor.Exit(queueLock);

			connection.Close();
		}
	}
}
