using System.Data;
using System.Threading;
using Server.Queries;
using ServiceStack.OrmLite;
using System;

namespace Server.Logic
{
	public class DeletingMessage
	{
		readonly Connections Connections;
		Locks Locks;

		public DeletingMessage(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Delete(string queueName, int messageId)
		{
			var connection = Connections.ConnectToInitializedQueue(queueName);
			var queueLock = Locks.TakeQueueLock(queueName);

			Monitor.Enter(queueLock);
			var messageDeleted = TryDelete(connection, messageId);
			connection.Close();
			Monitor.Exit(queueLock);

			if (!messageDeleted)
			{
				throw new ArgumentException("Provided message was not first message in queue");
			}
		}

		bool TryDelete(IDbConnection connection, int messageId)
		{
			var firstMessageInQueue = connection.FirstOrDefault(FirstMessageQuery.make(connection));
			if (firstMessageInQueue != null && firstMessageInQueue.Id == messageId)
			{
				firstMessageInQueue.Readed = true;
				connection.Update(firstMessageInQueue);
			}
			return firstMessageInQueue.Readed;
		}
	}
}
