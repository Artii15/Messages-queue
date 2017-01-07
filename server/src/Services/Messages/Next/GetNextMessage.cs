using ServiceStack.ServiceHost;

namespace Server.Services.Messages.Next
{
	[Route("/api/queues/{QueueName}/next", "GET")]
	public class GetNextMessage
	{
		public string QueueName { get; set; }
	}
}
