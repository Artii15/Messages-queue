using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/topics/{TopicName}/subscriptions", "delete")]
	public class DeleteSubscription
	{
		public string TopicName { get; set; }
	}
}
