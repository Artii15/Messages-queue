using ServiceStack.ServiceHost;

namespace Server.Services.Failures.Topics
{
	[Route("/failures/topics", "post")]
	public class TopicFailure
	{
		public string Name { get; set; }
		public string Cooperator { get; set; }
	}
}
