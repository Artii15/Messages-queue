using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/topics/{TopicName}/announcements/{AnnouncementId}", "delete")]
	public class DeleteAnnouncement
	{
		public string TopicName { get; set; }
		public int AnnouncementId { get; set; }
	}
}
