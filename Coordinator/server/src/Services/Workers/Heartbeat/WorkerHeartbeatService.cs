using ServiceStack.ServiceInterface;
using ServiceStack.OrmLite;
using ServiceStack.Common.Web;
using System.Net;

namespace Server
{
	public class WorkerHeartbeatService : Service
	{
		readonly RegisteringWorker RegisteringWorker;

		public WorkerHeartbeatService()
		{
			RegisteringWorker = new RegisteringWorker(Db);
			Db.CreateTableIfNotExists<Worker>();
		}

		public object Post(WorkerHeartbeat request)
		{
			RegisteringWorker.Register(request);
			return new HttpResult(new WorkerHeartbeatResponse(), HttpStatusCode.Created);
		}
	}
}
