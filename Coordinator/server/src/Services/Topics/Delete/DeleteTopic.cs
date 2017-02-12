using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/topics/{TopicName}", "delete")]
	public class DeleteTopic
	{
		public string TopicName { get; set; }
	}
}
