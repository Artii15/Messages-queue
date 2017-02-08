using ServiceStack.ServiceInterface;
using ServiceStack.OrmLite;

namespace Server
{
	public class RegisterWorkerService : Service
	{
		readonly RegisteringWorker RegisteringWorker;

		public RegisterWorkerService()
		{
			RegisteringWorker = new RegisteringWorker(Db);
			Db.CreateTableIfNotExists<Worker>();
		}

		public RegisterWorkerResponse Post(RegisterWorker request)
		{
			RegisteringWorker.Register(request);
			return new RegisterWorkerResponse();
		}

		public RegisterWorkerResponse Put(RegisterWorker request)
		{
			RegisteringWorker.Revive(request);
			return new RegisterWorkerResponse();
		}
	}
}
