using System;
using System.Collections.Concurrent;
using System.Data;
using Server.Entities;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class ReadingMessage
	{
		readonly ConcurrentDictionary<string, IDbConnectionFactory> Queues;

		public ReadingMessage(ConcurrentDictionary<string, IDbConnectionFactory> queues)
		{
			Queues = queues;
		}

		public string ReadNextFrom(string queueName)
		{
			IDbConnectionFactory queueConnectionFactory;
			if (Queues.TryGetValue(queueName, out queueConnectionFactory))
			{
				var connection = queueConnectionFactory.Open();
				var message = ReadMessage(connection);
				connection.Close();

				return message;
			}
			else
			{
				throw new ArgumentException("Specified queue not found");
			}
		}

		string ReadMessage(IDbConnection connection)
		{
			var query = connection.CreateExpression<QueueMessage>()
						  .Where(message => !message.Readed)
						  .OrderByDescending(message => message.Id);
			var messageToReturn = connection.FirstOrDefault(query);

			return messageToReturn.Content;
		}
	}
}
