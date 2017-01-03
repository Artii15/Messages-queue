using ServiceStack.ServiceHost;

namespace Server.Services.Queues.Create
{
	[Route("/api/queues", "POST")]
	public class CreateQueue
	{
		public string QueueName { get; set; }
	}
}