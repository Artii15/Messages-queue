using System.Collections.Generic;

namespace Server
{
	public class QueuesAndTopics
	{
		public Dictionary<string, Queue> Queues { get; set; } = new Dictionary<string, Queue>();
		public Dictionary<string, Topic> Topics { get; set; } = new Dictionary<string, Topic>();
	}
}
