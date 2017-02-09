using System;
using ServiceStack.ServiceHost;

namespace Server.Services.Subscriptions.Create
{
	[Route("/topics/{TopicName}/subscriptions", "post")]
	public class CreateSubscription
	{
		public string TopicName { get; set; }
		public int SubscriberId { get; set; }
		public DateTime? CreationTime { get; set; }
		public string Cooperator { get; set; }
	}
}
