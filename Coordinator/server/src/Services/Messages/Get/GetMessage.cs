using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/queues/{QueueName}/messages", "GET")]
	public class GetMessage
	{
		public string QueueName { get; set; }
	}
}
