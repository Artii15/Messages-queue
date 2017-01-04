using System.Collections.Generic;

namespace Server.Services.Queues.List
{
	public class ListQueuesResponse
	{
		public IEnumerable<string> Queues { get; set; }
	}
}
