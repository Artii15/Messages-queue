using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using System.Net;
using ServiceStack.Common;

namespace Server
{
	[Authenticate]
	public class GetAnnouncementService : Service
	{
		readonly GettingAnnouncement GettingAnnouncement;

		public GetAnnouncementService()
		{
			GettingAnnouncement = new GettingAnnouncement(Db);
		}

		public object Get(GetAnnouncement request)
		{
			Announcement announcement;
			try
			{
				var subscriberId = this.GetSession().UserAuthId.ToInt();
				announcement = GettingAnnouncement.Get(request, subscriberId);
			}
			catch (TopicNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Topic {request.TopicName} not exists");
			}
			catch (NoNewContentToGetException)
			{
				return new HttpError(HttpStatusCode.NoContent, "No new content");
			}
			catch (BadRequestException)
			{
				return new HttpError(HttpStatusCode.BadRequest, "BadRequest");
			}

			return new GetAnnouncementResponse()
			{
				Id = announcement.AnnouncementId,
				Content = announcement.Content
			};
		}
	}
}
