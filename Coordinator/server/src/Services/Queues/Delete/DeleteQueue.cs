using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/queues/{QueueName}", "delete")]
	public class DeleteQueue
	{
		public string QueueName { get; set; }
	}
}
