using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using Server.Entities;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class ReadingMessage
	{
		Connections Connections;
		readonly ConcurrentDictionary<string, object> QueuesLocks;

		public ReadingMessage(Connections connections,
		                      ConcurrentDictionary<string, object> queuesLocks)
		{
			Connections = connections;
			QueuesLocks = queuesLocks;
		}

		public string ReadNextFrom(string queueName)
		{
			var connection = Connections.ConnectToInitializedQueue(queueName);
			var queueLock = QueuesLocks.GetOrAdd(queueName, new object());
			var message = ReadMessage(connection, queueLock);
			connection.Close();

			return message;
		}

		string ReadMessage(IDbConnection connection, object queueLock)
		{
			Monitor.Enter(queueLock);

			var messageToReturn = ReadFromDb(connection);
			while (messageToReturn == null)
			{
				Monitor.Wait(queueLock);
				messageToReturn = ReadFromDb(connection);
			}
			Monitor.Exit(queueLock);

			return messageToReturn.Content;
		}

		QueueMessage ReadFromDb(IDbConnection connection)
		{
			var query = connection.CreateExpression<QueueMessage>()
						  .Where(message => !message.Readed)
						  .OrderByDescending(message => message.Id);
			return connection.FirstOrDefault(query);
		}
	}
}