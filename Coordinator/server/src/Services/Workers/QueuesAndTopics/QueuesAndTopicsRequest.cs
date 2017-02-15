using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/workers/queues-and-topics", "get")]
	[Restrict(EndpointAttributes.InternalNetworkAccess)]
	public class QueuesAndTopicsRequest
	{
		public int WorkerId { get; set; }
	}
}
