﻿using System.Data;
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

		public static Queue getQueueByName(IDbConnection dbConnection, string queueName)
		{
			var exp = dbConnection.CreateExpression<Queue>().Where(queue => queue.Name == queueName);
			return dbConnection.FirstOrDefault(exp);
		}
	}
}