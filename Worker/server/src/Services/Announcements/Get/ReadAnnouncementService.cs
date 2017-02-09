using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Announcements.Get
{
	public class ReadAnnouncementService: IService
	{
		ReadingAnnouncement ReadingAnnouncement;

		public ReadAnnouncementService(ReadingAnnouncement readingAnnouncement)
		{
			ReadingAnnouncement = readingAnnouncement;
		}

		public ReadAnnouncementResponse Get(ReadAnnouncement request)
		{
			var announcement = ReadingAnnouncement.Read(request);
			return new ReadAnnouncementResponse { Content = announcement.Content, AnnouncementId = announcement.Id };
		}
	}
}
