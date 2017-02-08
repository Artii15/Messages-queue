using ServiceStack.OrmLite;
using System.Collections.Concurrent;
using ServiceStack.OrmLite.Sqlite;

namespace Server.Logic
{
	public class Connections
	{
		ConcurrentDictionary<string, IDbConnectionFactory> Queues;

		public Connections()
		{
			Queues = new ConcurrentDictionary<string, IDbConnectionFactory>();
		}

		public IDbConnectionFactory ConnectToQueue(string queueName)
		{
			var queueStoragePath = $"queues/{queueName}.sqlite";
			return Queues.GetOrAdd(queueName,
			                       new OrmLiteConnectionFactory($"Data Source={queueStoragePath};Version=3;",
														 SqliteOrmLiteDialectProvider.Instance));
		}
	}
}
