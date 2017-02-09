using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/queues/{QueueName}/messages", "post")]
	public class CreateMessage
	{
		public string QueueName { get; set;}
		public string Content { get; set;}
	}
}
