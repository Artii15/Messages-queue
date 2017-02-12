using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/topics/{TopicName}/announcements", "post")]
	public class CreateAnnouncement
	{
		public string TopicName { get; set; }
		public string Content { get; set; }
	}
}
