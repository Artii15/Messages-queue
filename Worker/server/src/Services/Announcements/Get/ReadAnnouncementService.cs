using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Announcements.Get
{
	public class ReadAnnouncementService: Service
	{
		readonly ReadingAnnouncement ReadingAnnouncement;

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
