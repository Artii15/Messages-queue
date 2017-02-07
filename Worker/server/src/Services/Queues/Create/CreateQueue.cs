using ServiceStack.ServiceHost;

namespace Server.Services.Queues.Create
{
	[Route("/queues", "post")]
	public class CreateQueue
	{
		public string Name { get; set; }
		public string Cooperator { get; set; }
	}
}