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
			if (Encrypt.EncryptToken(request.Time, request.Id, request.Address, request.Token))
				using (IDbTransaction transaction = DBConnection.OpenTransaction())
				{
					if (!WorkerQueries.WorkerExists(DBConnection, request.Id))
						AddNewWorker(request);
					else
						Revive(request);
					transaction.Commit();
				}
			else
				throw new BadRequestException();
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
