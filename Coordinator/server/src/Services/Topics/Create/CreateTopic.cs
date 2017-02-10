using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/topics", "post")]
	public class CreateTopic
	{
		public string Name { get; set; }
	}
}
