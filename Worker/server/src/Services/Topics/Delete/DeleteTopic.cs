using ServiceStack.ServiceHost;

namespace Server.Services.Topics.Delete
{
	[Route("/topics/{TopicName}", "delete")]
	public class DeleteTopic
	{
		public string TopicName { get; set; }
		public string Cooperator { get; set; }
	}
}
