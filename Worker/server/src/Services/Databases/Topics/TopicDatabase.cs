using System.IO;
using ServiceStack.ServiceHost;

namespace Server.Services.Databases.Topics
{
	[Route("/databases/topics/{Name}", "put")]
	public class TopicDatabase: IRequiresRequestStream
	{
		public string Name { get; set; }
		public Stream RequestStream { get; set; }
	}
}
