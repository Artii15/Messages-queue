using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Announcements.Delete
{
	public class DeleteAnnouncementService: Service
	{
		readonly DeletingAnnouncement DeletingAnnouncement;

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
