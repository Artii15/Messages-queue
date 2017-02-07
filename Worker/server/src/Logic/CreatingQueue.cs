using System.Collections.Concurrent;
using Server.Entities;
using Server.Services.CreatingQueue;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;

namespace Server.Logic
{
	public class CreatingQueue
	{
		readonly ConcurrentDictionary<string, IDbConnectionFactory> Queues;
		readonly IDbConnectionFactory SystemDb;

		public void Create(CreateQueue request)
		{
			var queueStoragePath = $"queues/{request.Name}.sqlite";
			var queueDbConn = Queues.GetOrAdd(request.Name, 
			                new OrmLiteConnectionFactory($"Data Source={queueStoragePath};Version=3;", 
			                                             SqliteOrmLiteDialectProvider.Instance)).Open();
			var systemDbConn = SystemDb.Open();
			systemDbConn.Insert(new Cooperator { Address=request.Cooperator });


			queueDbConn.Close();
			systemDbConn.Close();
		}
	}
}