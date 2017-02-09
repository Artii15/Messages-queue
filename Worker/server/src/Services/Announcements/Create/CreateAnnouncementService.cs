using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Announcements.Create
{
	public class CreateAnnouncementService: IService
	{
		readonly CreatingAnnouncement CreatingAnnouncement;

		public CreateAnnouncementService(CreatingAnnouncement creatingAnnouncement)
		{
			CreatingAnnouncement = creatingAnnouncement;
		}

		public CreateAnnouncementResponse Post(CreateAnnouncement request)
		{
			CreatingAnnouncement.Create(request);
			return new CreateAnnouncementResponse();
		}
	}
}
