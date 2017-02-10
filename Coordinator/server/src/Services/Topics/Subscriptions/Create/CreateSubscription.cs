using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/topics/{TopicName}/subscriptions", "post")]
	public class CreateSubscription
	{
		public string TopicName { get; set; }
	}
}
