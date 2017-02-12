using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Net;
using System.Data;
using ServiceStack.Common;

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
				var subscriberId = this.GetSession().UserAuthId.ToInt();
				DeletingAnnouncement.Delete(request, subscriberId);
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
