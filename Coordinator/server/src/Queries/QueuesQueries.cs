using System.Linq;
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

		public static Queue CreateQueue(IDbConnection dbConnection, string name, long worker, long coworker)
		{
			dbConnection.Insert(new Queue { Name = name, Worker = worker, Cooperator = coworker });
			return getQueueByName(dbConnection, name);
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

		public static Dictionary<string, RecoveryQueue> GetWorkerQueues(IDbConnection dbConnection, long workerId)
		{
			var qList = dbConnection.Select<RecoveryQueue>("Select Name, Address as Cooperator " +
			                                               $"from Queue q join Worker w on q.Cooperator = w.Id where q.worker = {workerId}");
			var qList2 = dbConnection.Select<RecoveryQueue>("Select Name, Address as Cooperator " +
														   $"from Queue q join Worker w on q.Worker = w.Id where q.cooperator = {workerId}");
			return qList.Concat(qList2).ToDictionary(x => x.Name, x => x);
		}

		public static void swapWorkers(IDbConnection dbConnection, Queue queue)
		{
			dbConnection.Update(new Queue { Id = queue.Id, Name = queue.Name, Worker = queue.Cooperator, Cooperator = queue.Worker },
									q => q.Id == queue.Id);
		}
	}
}
