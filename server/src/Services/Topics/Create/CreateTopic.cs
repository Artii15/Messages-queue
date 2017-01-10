using ServiceStack.ServiceHost;

namespace Server.Services.Topics.Create
{
	[Route("/api/queues/{QueueName}/topics", "POST")]
	public class CreateTopic
	{
		public string QueueName { get; set; }
		public string TopicName { get; set; }
	}
}
