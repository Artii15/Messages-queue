using ServiceStack.ServiceInterface;
using ServiceStack.OrmLite;
using ServiceStack.Common.Web;
using System.Net;

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

		public object Post(RegisterWorker request)
		{
			RegisteringWorker.Register(request);
			return new HttpResult(new RegisterWorkerResponse(), HttpStatusCode.Created);
		}
	}
}
