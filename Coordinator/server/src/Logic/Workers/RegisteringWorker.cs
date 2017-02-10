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

		public void Register(RegisterWorker request)
		{
			if (request.Id == null)
				AddNewWorker(request.Address);
			else
				Revive(request);
		}

		void Revive(RegisterWorker request)
		{
			if (WorkerQueries.WorkerExists(DBConnection, request.Id.Value))
				WorkerQueries.ReviveWorker(DBConnection, request);
			else
				AddNewWorker(request.Address);

		}

		void AddNewWorker(string address)
		{
			var worker = new Worker()
			{
				Address = address,
				Alive = true
			};
			WorkerQueries.AddNewWorker(DBConnection, worker);
		}
			
	}
}
