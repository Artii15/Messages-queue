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
				PropagateRequestToCo(request);		
				CreateQueueFile(request.Name);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
		}

		void CreateQueueFile(string queueName)
		{
			var queueStoragePath = $"queues/{queueName}.sqlite";
			var queueDbConn = Queues.GetOrAdd(queueName,
							new OrmLiteConnectionFactory($"Data Source={queueStoragePath};Version=3;",
														 SqliteOrmLiteDialectProvider.Instance)).Open();
			queueDbConn.CreateTableIfNotExists<QueueMessage>();
			queueDbConn.Close();
		}

		void PropagateRequestToCo(CreateQueue request)
		{

		}
	}
}