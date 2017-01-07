using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/api/queues/{QueueName}/next")]
	public class GetNextMessage
	{
		public string QueueName { get; set; }
	}
}
