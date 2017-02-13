using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/workers/queues-and-topics", "get")]
	[Restrict(InternalOnly = true)]
	public class QueuesAndTopicsRequest
	{
		public int WorkerId { get; set; }
	}
}
