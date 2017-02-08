using System.Data;
using System.Threading;
using Server.Entities;
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

		public string ReadNextFrom(string queueName)
		{
			var connection = Connections.ConnectToInitializedQueue(queueName);
			var queueLock = Locks.TakeQueueLock(queueName);
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