using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Net;
using System.Data;

namespace Server
{
	[Authenticate]
	public class DeleteAnnouncementService : Service
	{
		readonly DeletingAnnouncement DeletingAnnouncement;

		public DeleteAnnouncementService()
		{
			DeletingAnnouncement = new DeletingAnnouncement(Db);
		}

		public object Delete(DeleteAnnouncement request)
		{
			IDbTransaction transaction = Db.OpenTransaction();
			try
			{
				DeletingAnnouncement.Delete(request);
			}
			catch (TopicNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Topic {request.TopicName} not exists");
			}
			finally
			{
				transaction.Commit();
			}

			return new HttpResult(new DeleteAnnouncementResponse(), HttpStatusCode.NoContent);
		}
	}
}
