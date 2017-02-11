using ServiceStack.ServiceHost;

namespace Server.Services.Databases.Topics
{
	[Route("databases/topics/{Name}", "put")]
	public class TopicDatabase
	{
		public string Name { get; set; }
		public byte[] DatabaseFile { get; set; }
	}
}
