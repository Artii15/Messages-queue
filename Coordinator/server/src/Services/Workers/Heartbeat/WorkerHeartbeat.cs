using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/workers/heartbeat", "post")]
	[Restrict(EndpointAttributes.InternalNetworkAccess)]
	public class WorkerHeartbeat
	{
		public int Id { get; set; }
		public string Address { get; set; }
	}
}
