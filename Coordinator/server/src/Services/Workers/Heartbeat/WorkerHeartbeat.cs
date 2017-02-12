using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/workers/heartbeat", "post")]
	[Restrict(InternalOnly = true)]
	public class WorkerHeartbeat
	{
		public int Id { get; set; }
		public string Address { get; set; }
	}
}
