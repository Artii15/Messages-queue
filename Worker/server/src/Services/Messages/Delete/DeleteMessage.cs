using ServiceStack.ServiceHost;

namespace Server.Services.Messages.Delete
{
	[Route("/queues/{QueueName}/messages/{MessageId}")]
	public class DeleteMessage
	{
		public string QueueName { get; set; }
		public int MessageId { get; set; }
	}
}