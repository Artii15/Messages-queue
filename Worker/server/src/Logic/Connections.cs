using ServiceStack.OrmLite;
using System.Collections.Concurrent;
using ServiceStack.OrmLite.Sqlite;
using Server.Entities;
using System.Data;
using System;

namespace Server.Logic
{
	public class Connections
	{
		readonly ConcurrentDictionary<string, IDbConnectionFactory> Queues;

		public Connections()
		{
			Queues = new ConcurrentDictionary<string, IDbConnectionFactory>();
		}

		public IDbConnection ConnectToInitializedQueue(string queueName)
		{
			var connection = ConnectToQueue(queueName);
			if(connection.TableExists(typeof(QueueMessage).ToString()))
			{
				return connection;
			}
			throw new ArgumentException($"Queue {queueName} not exists");
		}

		public IDbConnection ConnectToQueue(string queueName)
		{
			var queueStoragePath = $"queues/{queueName}.sqlite";
			return Queues.GetOrAdd(queueName,
			                       new OrmLiteConnectionFactory($"Data Source={queueStoragePath};Version=3;",
			                                                    SqliteOrmLiteDialectProvider.Instance)).Open();
		}
	}
}
