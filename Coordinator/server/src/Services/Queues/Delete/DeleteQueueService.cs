using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Net;
using System.Data;

namespace Server
{
	[Authenticate]
	public class DeleteQueueService : Service
	{
		readonly DeletingQueue DeletingQueue;

		public DeleteQueueService()
		{
			DeletingQueue = new DeletingQueue(Db);
		}

		[RequiredPermission("create")]
		public object Delete(DeleteQueue request)
		{
			IDbTransaction transaction = Db.OpenTransaction();
			try
			{
				DeletingQueue.Delete(request);
			}
			catch (QueueNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Queue {request.QueueName} not exists");
			}
			catch (BadRequestException)
			{
				return new HttpError(HttpStatusCode.BadRequest, "BadRequest");
			}
			finally
			{
				transaction.Commit();
			}

			return new HttpResult(new DeleteQueueResponse(), HttpStatusCode.NoContent);
		}
	}
}
