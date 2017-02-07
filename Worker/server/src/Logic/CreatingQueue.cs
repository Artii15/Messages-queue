using System;
using System.Collections.Concurrent;
using Server.Entities;
using Server.Services.Queues.Create;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;

namespace Server.Logic
{
	public class CreatingQueue
	{
		readonly ConcurrentDictionary<string, IDbConnectionFactory> Queues;

		public CreatingQueue(ConcurrentDictionary<string, IDbConnectionFactory> queues)
		{
			Queues = queues;
		}

		public void Create(CreateQueue request)
		{
			try
			{
				var queueStoragePath = $"queues/{request.Name}.sqlite";
				var queueDbConn = Queues.GetOrAdd(request.Name,
								new OrmLiteConnectionFactory($"Data Source={queueStoragePath};Version=3;",
															 SqliteOrmLiteDialectProvider.Instance)).Open();
				queueDbConn.CreateTableIfNotExists<QueueMessage>();
				queueDbConn.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
		}
	}
}