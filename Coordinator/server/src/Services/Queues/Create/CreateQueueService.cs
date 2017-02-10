using ServiceStack.Common.Web;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Net;

namespace Server
{
	[Authenticate]
	public class CreateQueueService : Service
	{
		readonly CreatingQueue CreatingQueue;

		public CreateQueueService()
		{
			CreatingQueue = new CreatingQueue(Db);
			Db.CreateTableIfNotExists<Queue>();
		}

		public object Post(CreateQueue request)
		{
			try
			{
				CreatingQueue.Create(request);
			}
			catch (QueueAlreadyExistsException)
			{
				return new HttpError(HttpStatusCode.Conflict, $"Queue {request.Name} already exists");
			}

			return new HttpResult(new CreateQueueResponse())
			{
				StatusCode = HttpStatusCode.Created,
				Headers =
							   {
					{HttpHeaders.Location, base.Request.AbsoluteUri.CombineWith(request.Name)}
							   }
			};
		}
	}
}
