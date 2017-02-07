using ServiceStack.ServiceHost;

namespace Server.Services.Messages.Create
{
	[Route("/queues/{QueueName}/messages", "post")]
	public class CreateMessage
	{
		public string QueueName { get; set; }
		public string Content { get; set; }
		public int Author { get; set; }
		public string Cooperator { get; set; }
	}
}
