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
	}
}
