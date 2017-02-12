using System.Collections.Generic;
using System.Data;
using ServiceStack.OrmLite;

namespace Server
{
	public static class QueuesQueries
	{
		public static bool QueueExists(IDbConnection dbConnection, string name)
		{
			return dbConnection.Select<Queue>(queue => queue.Name == name).Count != 0;
		}

		public static void CreateQueue(IDbConnection dbConnection, string name, long worker, long coworker)
		{
			dbConnection.Insert(new Queue { Name = name, Worker = worker, Cooperator = coworker });
		}

		public static void DeleteQueue(IDbConnection dbConnection, string name)
		{
			dbConnection.Delete<Queue>(queue => queue.Name == name);
		}

		public static Queue getQueueByName(IDbConnection dbConnection, string queueName)
		{
			var exp = dbConnection.CreateExpression<Queue>().Where(queue => queue.Name == queueName);
			return dbConnection.FirstOrDefault(exp);
		}
		public static List<string> getQueuesNames(IDbConnection dbConnection)
		{
			return new List<string>(dbConnection.List<string>("select Name from Queue"));
		}
	}
}
