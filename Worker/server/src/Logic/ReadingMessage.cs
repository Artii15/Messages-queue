using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using Server.Entities;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class ReadingMessage
	{
		readonly ConcurrentDictionary<string, IDbConnectionFactory> Queues;
		readonly ConcurrentDictionary<string, object> QueuesLocks;

		public ReadingMessage(ConcurrentDictionary<string, IDbConnectionFactory> queues,
		                      ConcurrentDictionary<string, object> queuesLocks)
		{
			Queues = queues;
			QueuesLocks = queuesLocks;
		}

		public string ReadNextFrom(string queueName)
		{
			IDbConnectionFactory queueConnectionFactory;
			if (Queues.TryGetValue(queueName, out queueConnectionFactory))
			{
				var connection = queueConnectionFactory.Open();
				var queueLock = QueuesLocks.GetOrAdd(queueName, new object());
				var message = ReadMessage(connection, queueLock);
				connection.Close();

				return message;
			}
			throw new ArgumentException("Specified queue not found");
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