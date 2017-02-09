using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/queues", "post")]
	public class CreateQueue
	{
		public string Name { get; set;}
	}
}
