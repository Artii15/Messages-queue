using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Net;
using System.Data;

namespace Server
{
	[Authenticate]
	public class DeleteMessageService : Service
	{
		readonly DeletingMessage DeletingMessage;

		public DeleteMessageService()
		{
			DeletingMessage = new DeletingMessage(Db);
		}

		public object Delete(DeleteMessage request)
		{
			IDbTransaction transaction = Db.OpenTransaction();
			try
			{
				DeletingMessage.Delete(request);
			}
			catch (QueueNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Queue {request.QueueName} not exists");
			}
			finally
			{
				transaction.Commit();
			}

			return new HttpResult(new DeleteMessageResponse(), HttpStatusCode.NoContent);
		}
	}
}
