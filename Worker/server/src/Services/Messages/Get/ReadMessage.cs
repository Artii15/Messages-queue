using ServiceStack.ServiceHost;

namespace Server.Services.Messages.Get
{
	[Route("/queues/{QueueName}/messages", "GET")]
	public class ReadMessage
	{
		public string QueueName { get; set; }
	}
}
