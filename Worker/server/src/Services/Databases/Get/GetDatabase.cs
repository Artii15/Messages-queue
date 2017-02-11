using ServiceStack.ServiceHost;

namespace Server.Services.Databases.Get
{
	[Route("databases/{Category}/{Name}", "get")]
	public class GetDatabase
	{
		public string Category { get; set; }
		public string Name { get; set; }
	}
}
