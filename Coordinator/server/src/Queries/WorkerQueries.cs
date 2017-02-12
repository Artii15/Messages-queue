using System;
using System.Data;
using ServiceStack.OrmLite;

namespace Server
{
	public static class WorkerQueries
	{
		public static long GetWorkersCount(IDbConnection dbConnection)
		{
			return dbConnection.Count<Worker>();
		}

		public static Worker GetWorker(IDbConnection dbConnection, long position)
		{
			var exp = dbConnection.CreateExpression<Worker>()
								  .OrderBy(worker => worker.Id)
			                      .Limit(skip: (int)position, rows: 1);
			return dbConnection.FirstOrDefault(exp);
		}

		public static Worker GetWorkerById(IDbConnection dbConnection, long id)
		{
			var exp = dbConnection.CreateExpression<Worker>()
			                      .Where(worker => worker.Id == id);
			return dbConnection.FirstOrDefault(exp);
		}

		public static bool IsWorkerAlive(IDbConnection dbConnection, long id)
		{
			var exp = dbConnection.CreateExpression<Worker>()
			                      .Where(worker => worker.Id == id);
			return dbConnection.FirstOrDefault(exp).Alive;
		}

		public static void AddNewWorker(IDbConnection dbConnection, Worker worker)
		{
			dbConnection.Insert(worker);
		}

		public static void ReviveWorker(IDbConnection dbConnection, WorkerHeartbeat request)
		{
			dbConnection.Update(new Worker { Id = request.Id, Address = request.Address, Alive = true, LastHeartbeat = DateTime.UtcNow },
									w => w.Id == request.Id);
		}

		public static bool WorkerExists(IDbConnection dbConnection, long workerId)
		{
			return dbConnection.Select<Worker>(w => w.Id == workerId).Count == 1;
		}
	}
}
