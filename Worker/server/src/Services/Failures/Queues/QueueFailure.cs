using ServiceStack.ServiceHost;

namespace Server.Services.Failures.Queues
{
	[Route("/failures/queues", "post")]
	public class QueueFailure
	{
		public string Name { get; set; }
	}
}
