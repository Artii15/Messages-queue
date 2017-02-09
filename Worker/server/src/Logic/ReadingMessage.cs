using System.Data;
using System.Threading;
using Server.Entities;
using Server.Queries;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class ReadingMessage
	{
		Connections Connections;
		readonly Locks Locks;

		public ReadingMessage(Connections connections,
		                      Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public QueueMessage ReadNextFrom(string queueName)
		{
			var connection = Connections.ConnectToInitializedQueue(queueName);
			var queueLock = Locks.TakeQueueLock(queueName);
			var message = ReadMessage(connection, queueLock);
			connection.Close();

			return message;
		}

		QueueMessage ReadMessage(IDbConnection connection, object queueLock)
		{
			Monitor.Enter(queueLock);

			var messageToReturn = ReadFromDb(connection);
			while (messageToReturn == null)
			{
				Monitor.Wait(queueLock);
				messageToReturn = ReadFromDb(connection);
			}
			Monitor.Exit(queueLock);

			return messageToReturn;
		}

		QueueMessage ReadFromDb(IDbConnection connection)
		{
			return connection.FirstOrDefault(FirstMessageQuery.make(connection));
		}
	}
}