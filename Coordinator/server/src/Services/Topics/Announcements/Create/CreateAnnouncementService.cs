using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using System.Net;

namespace Server
{
	[Authenticate]
	public class CreateAnnouncementService : Service
	{
		readonly CreatingAnnouncement CreatingAnnouncement;

		public CreateAnnouncementService()
		{
			CreatingAnnouncement = new CreatingAnnouncement(Db);
		}

		[RequiredPermission("write")]
		public object Post(CreateAnnouncement request)
		{
			try
			{
				CreatingAnnouncement.Create(request);
			}
			catch (TopicNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Topic {request.TopicName} not exists");
			}

			return new HttpResult(new CreateAnnouncementResponse(), HttpStatusCode.Created);
		}
	}
}
