using System.IO;
using ServiceStack.ServiceHost;

namespace Server.Services.Databases.Queues
{
	[Route("/databases/queues/{Name}", "put")]
	public class QueueDatabase: IRequiresRequestStream
	{
		public string Name { get; set; }
		public Stream RequestStream { get; set; }
	}
}