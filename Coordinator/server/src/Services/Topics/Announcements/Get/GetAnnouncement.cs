using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/topics/{TopicName}/announcements", "get")]
	public class GetAnnouncement
	{
		public string TopicName { get; set; }
	}
}
