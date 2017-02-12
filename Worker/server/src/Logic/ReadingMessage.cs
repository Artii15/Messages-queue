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
			using (var connection = Connections.ConnectToInitializedQueue(queueName))
			{
				return ReadMessage(connection, Locks.TakeQueueLock(queueName));
			}
		}

		QueueMessage ReadMessage(IDbConnection connection, object queueLock)
		{
			lock (queueLock)
			{
				var messageToReturn = ReadFromDb(connection);
				while (messageToReturn == null)
				{
					Monitor.Wait(queueLock);
					messageToReturn = ReadFromDb(connection);
				}

				return messageToReturn;
			}
		}

		QueueMessage ReadFromDb(IDbConnection connection)
		{
			return connection.FirstOrDefault(FirstMessageQuery.make(connection));
		}
	}
}