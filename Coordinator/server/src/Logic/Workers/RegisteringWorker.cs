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

		public void Revive(RegisterWorker request)
		{ 
			if (DBConnection.Exists<Worker>(new { Id = request.Id }))
			{

				DBConnection.Update<Worker>(new { Address = request.Address }, w => w.Id == request.Id);
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
