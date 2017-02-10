using ServiceStack.ServiceHost;

namespace Server.Services.Announcements.Delete
{
	[Route("/topics/{TopicName}/announcements/{AnnouncementId}", "delete")]
	public class DeleteAnnouncement
	{
		public string TopicName { get; set; }
		public int AnnouncementId { get; set; }
		public int SubscriberId { get; set; }
		public string Cooperator { get; set; }
	}
}