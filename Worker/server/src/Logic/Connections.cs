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
		readonly ConcurrentDictionary<string, IDbConnectionFactory> Topics;

		public Connections()
		{
			Queues = new ConcurrentDictionary<string, IDbConnectionFactory>();
		}

		public IDbConnection ConnectToInitializedQueue(string queueName)
		{
			return Verify(ConnectToQueue(queueName), typeof(QueueMessage).Name);
		}

		public IDbConnection ConnectToQueue(string queueName)
		{
			return Connect(Queues, "queues", queueName);
		}

		public IDbConnection ConnectToInitializedTopic(string topicName)
		{
			return Verify(ConnectToTopic(topicName), typeof(Announcement).Name);
		}

		public IDbConnection ConnectToTopic(string topicName)
		{
			return Connect(Topics, "topics", topicName);
		}

		IDbConnection Verify(IDbConnection connection, string requiredTable)
		{
			if (!connection.TableExists(requiredTable))
			{
				connection.Close();
				throw new ArgumentException($"{requiredTable} not exists");
			}
			return connection;
		}

		IDbConnection Connect(ConcurrentDictionary<string, IDbConnectionFactory> collection, string directory, string dbName)
		{
			return collection.GetOrAdd(dbName, _ => MakeConnectionFactory(directory, dbName)).Open();
		}

		OrmLiteConnectionFactory MakeConnectionFactory(string dbDirectory, string dbName)
		{
			return new OrmLiteConnectionFactory($"Data Source={dbDirectory}/{dbName}.sqlite;Version=3;",
												SqliteOrmLiteDialectProvider.Instance);
		}
	}
}