using System;
using System.Data;
using ServiceStack.OrmLite;

namespace Server
{
	public static class WorkerQueries
	{
		static readonly double HeartBeatThreshold = 12; //seconds

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
			                      .Where(w => w.Id == id);
			var worker = dbConnection.FirstOrDefault(exp);
			if (worker.LastHeartbeat.AddSeconds(HeartBeatThreshold) >= DateTime.UtcNow)
				return true;
			else
			{
				updateWorkerLiveliness(dbConnection, worker);
				return false;
			}
		}

		static void updateWorkerLiveliness(IDbConnection dbConnection, Worker worker)
		{
			dbConnection.Update(new Worker { Id = worker.Id, Address = worker.Address, Alive = false, LastHeartbeat = worker.LastHeartbeat },
									w => w.Id == worker.Id);
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
