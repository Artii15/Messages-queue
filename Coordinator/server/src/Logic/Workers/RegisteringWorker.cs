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

		public void Register(RegisterWorker request)
		{
			if (request.Id == null)
			{
				AddNewWorker(request.Address);
			}
		}

		void AddNewWorker(string address)
		{
			var worker = new Worker()
			{
				Address = address,
				Alive = true
			};
			DBConnection.Insert(worker);
		}
			
	}
}
