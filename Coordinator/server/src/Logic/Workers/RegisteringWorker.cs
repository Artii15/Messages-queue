using System;
using System.Data;
using ServiceStack.OrmLite;

namespace Server
{
	public class RegisteringWorker
	{
		readonly IDbConnection DBConnection;

		public RegisteringWorker(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Register(WorkerHeartbeat request)
		{
			using (IDbTransaction transaction = DBConnection.OpenTransaction())
			{
				if (!WorkerQueries.WorkerExists(DBConnection, request.Id))
					AddNewWorker(request);
				else
					Revive(request);
				transaction.Commit();
			}
		}

		void Revive(WorkerHeartbeat request)
		{
			WorkerQueries.ReviveWorker(DBConnection, request);
		}

		void AddNewWorker(WorkerHeartbeat request)
		{
			var worker = new Worker()
			{
				Id = request.Id,
				Address = request.Address,
				Alive = true,
				LastHeartbeat = DateTime.UtcNow
			};
			WorkerQueries.AddNewWorker(DBConnection, worker);
		}
			
	}
}
