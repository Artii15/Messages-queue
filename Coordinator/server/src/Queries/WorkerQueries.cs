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
	}
}
