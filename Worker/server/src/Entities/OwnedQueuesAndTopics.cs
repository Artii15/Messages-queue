using System.Collections.Generic;

namespace Server.Entities
{
	public class OwnedQueuesAndTopics
	{
		public Dictionary<string, OwnedQueue> OwnedQueues { get; set; } = new Dictionary<string, OwnedQueue>();
		public Dictionary<string, OwnedTopic> OwnedTopics { get; set; } = new Dictionary<string, OwnedTopic>();
	}
}
