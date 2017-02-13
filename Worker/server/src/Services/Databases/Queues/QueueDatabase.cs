using ServiceStack.ServiceHost;

namespace Server.Services.Databases.Queues
{
	[Route("/databases/queues/{Name}", "put")]
	public class QueueDatabase
	{
		public string Name { get; set; }
		public byte[] DatabaseFile { get; set; }
	}
}