using ServiceStack.ServiceHost;

namespace Server.Services.Topics.Create
{
	[Route("/topics", "post")]
	public class CreateTopic
	{
		public string Name { get; set; }
		public string Cooperator { get; set; }
	}
}
