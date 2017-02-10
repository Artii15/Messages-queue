using ServiceStack.ServiceHost;

namespace Server.Services.Topics.Delete
{
	[Route("/topics/{TopicName}", "delete")]
	public class DeleteTopic
	{
		public string TopicName;
		public string Cooperator;
	}
}
