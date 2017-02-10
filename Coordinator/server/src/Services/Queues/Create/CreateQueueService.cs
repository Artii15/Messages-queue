using ServiceStack.Common.Web;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Net;
using System.Data;

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
			IDbTransaction transaction = Db.OpenTransaction();
			try
			{
				CreatingQueue.Create(request);
			}
			catch (QueueAlreadyExistsException)
			{
				return new HttpError(HttpStatusCode.Conflict, $"Queue {request.Name} already exists");
			}
			finally
			{
				transaction.Commit();
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
