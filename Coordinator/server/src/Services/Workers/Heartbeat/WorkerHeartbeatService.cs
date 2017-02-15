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
			try
			{
				RegisteringWorker.Register(request);
			}
			catch (BadRequestException)
			{
				return new HttpError(HttpStatusCode.BadRequest, "BadRequest");
			}

			return new HttpResult(new WorkerHeartbeatResponse(), HttpStatusCode.Created);
		}
	}
}
