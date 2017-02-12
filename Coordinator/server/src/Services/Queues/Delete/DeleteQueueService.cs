using ServiceStack.Common.Web;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Net;
using System.Data;

namespace Server
{
	public class DeleteQueueService : Service
	{
		readonly DeletingQueue DeletingQueue;

		public DeleteQueueService()
		{
			DeletingQueue = new DeletingQueue(Db);
		}

		public object Post(CreateQueue request)
		{
			IDbTransaction transaction = Db.OpenTransaction();
			try
			{
				DeletingQueue.Delete(request);
			}
			catch (QueueNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Queue {request.Name} not exists");
			}
			finally
			{
				transaction.Commit();
			}

			return new HttpResult(new DeleteQueueResponse(), HttpStatusCode.NoContent);
		}
	}
}
