using ServiceStack.ServiceHost;

namespace Server.Services.Announcements.Get
{
	[Route("/topics/{TopicName}/announcements", "get")]
	public class ReadAnnouncement
	{
		public string TopicName { get; set; }
		public string SubscriberId { get; set; }
	}
}
