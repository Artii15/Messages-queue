using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/workers/queues-and-topics", "get")]
	public class QueuesAndTopicsRequest
	{
		public int WorkerId { get; set; }
		public long Time { get; set; }
		public string Token { get; set; }
	}
}
