using System.Collections.Concurrent;

namespace Server.Entities
{
	public class QueuesAndTopics
	{
		public ConcurrentDictionary<string, Queue> Queues { get; set; } = new ConcurrentDictionary<string, Queue>();
		public ConcurrentDictionary<string, Topic> Topics { get; set; } = new ConcurrentDictionary<string, Topic>();
	}
}
