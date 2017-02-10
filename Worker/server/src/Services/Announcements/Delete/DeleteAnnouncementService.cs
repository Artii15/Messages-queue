using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Announcements.Delete
{
	public class DeleteAnnouncementService: IService
	{
		DeletingAnnouncement DeletingAnnouncement;

		public DeleteAnnouncementService(DeletingAnnouncement deletingAnnouncement)
		{
			DeletingAnnouncement = deletingAnnouncement;
		}

		public DeleteAnnouncementResponse Delete(DeleteAnnouncement request)
		{
			DeletingAnnouncement.Delete(request);
			return new DeleteAnnouncementResponse();
		}
	}
}
