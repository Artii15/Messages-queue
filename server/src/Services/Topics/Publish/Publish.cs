using ServiceStack.ServiceHost;

namespace Server.Services.Topics.Publish
{
	[Route("/api/queues/{QueueName}/topics/{TopicName}/publications", "POST")]
	public class Publish
	{
		public string QueueName { get; set; }
		public string TopicName { get; set; }
		public string Message { get; set; }
	}
}
