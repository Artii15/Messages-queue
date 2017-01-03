using System;
using ServiceStack.ServiceHost;

namespace Server.Services.QueuesService
{
	[Route ("/api/queues", "GET POST")]
	public class QueuesRequest : IReturn<QueuesResponse>
	{
		
	}
}

