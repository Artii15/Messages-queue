using ServiceStack.ServiceHost;

namespace Server.Services.CreatingQueue
{
	[Route("/queues", "post")]
	public class CreateQueue
	{
		public string Name { get; set; }
		public string Cooperator { get; set; }
		public string AmIPrimary { get; set; }
	}
}
