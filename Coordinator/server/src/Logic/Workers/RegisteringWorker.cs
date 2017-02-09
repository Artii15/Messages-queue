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
			else
			{
				Revive(request);
			}
		}

		void Revive(RegisterWorker request)
		{ 
			if (DBConnection.Select<Worker>(w => w.Id == request.Id).Count == 1)
			{

				DBConnection.Update(new Worker{Id = request.Id.Value, Address = request.Address, Alive = true }, 
				                    w => w.Id == request.Id);
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
