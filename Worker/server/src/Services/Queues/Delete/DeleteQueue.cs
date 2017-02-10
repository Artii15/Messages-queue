using ServiceStack.ServiceHost;

namespace Server.Services.Queues.Delete
{
	[Route("/queues/{QueueName}")]
	public class DeleteQueue
	{
		public string QueueName { get; set; }
		public string Cooperator { get; set; }
	}
}
