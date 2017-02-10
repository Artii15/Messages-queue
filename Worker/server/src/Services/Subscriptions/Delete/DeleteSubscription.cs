using ServiceStack.ServiceHost;

namespace Server.Services.Subscriptions.Delete
{
	[Route("/topics/{TopicName}/subscriptions/{SubscriberId}", "delete")]
	public class DeleteSubscription
	{
		public string TopicName { get; set; }
		public int SubscriberId { get; set; }
		public string Cooperator { get; set; }
	}
}
