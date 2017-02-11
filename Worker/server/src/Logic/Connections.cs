using ServiceStack.OrmLite;
using System.Collections.Concurrent;
using ServiceStack.OrmLite.Sqlite;
using Server.Entities;
using System.Data;
using System;
using System.IO;

namespace Server.Logic
{
	public class Connections
	{
		readonly ConcurrentDictionary<string, IDbConnectionFactory> Queues;
		readonly ConcurrentDictionary<string, IDbConnectionFactory> Topics;

		public const string QueuesDir = "queues";
		public const string TopicsDir = "topics";

		public Connections()
		{
			Queues = new ConcurrentDictionary<string, IDbConnectionFactory>();
			Topics = new ConcurrentDictionary<string, IDbConnectionFactory>();
		}

		public IDbConnection ConnectToInitializedQueue(string queueName)
		{
			return Verify(ConnectToQueue(queueName), typeof(QueueMessage).Name);
		}

		public IDbConnection ConnectToQueue(string queueName)
		{
			return Connect(Queues, QueuesDir, queueName);
		}

		public IDbConnection ConnectToInitializedTopic(string topicName)
		{
			return Verify(ConnectToTopic(topicName), typeof(Announcement).Name);
		}

		public IDbConnection ConnectToTopic(string topicName)
		{
			return Connect(Topics, TopicsDir, topicName);
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
			return new OrmLiteConnectionFactory($"Data Source={PathToDbFile(dbDirectory, dbName)};Version=3;",
												SqliteOrmLiteDialectProvider.Instance);
		}

		public void RemoveTopic(string topicName)
		{
			Remove(TopicsDir, topicName);
		}

		public void RemoveQueue(string queueName)
		{
			Remove(QueuesDir, queueName);
		}

		void Remove(string dbDirectory, string dbName)
		{
			File.Delete(PathToDbFile(dbDirectory, dbName));
		}

		public static string PathToDbFile(string dbDir, string dbName)
		{
			return $"{dbDir}/{dbName}.sqlite";
		}
	}
}