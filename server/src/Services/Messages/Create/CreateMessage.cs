using ServiceStack.ServiceHost;

namespace Server.Services.Messages.Create
{
	[Route("api/queues/{queueName}/messages", "POST")]
	public class CreateMessage
	{
		public string Content;
	}
}