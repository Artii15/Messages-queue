using System.Collections.Generic;

namespace Server
{
	public class QueuesAndTopics
	{
		public Dictionary<string, RecoveryQueue> Queues { get; set; } = new Dictionary<string, RecoveryQueue>();
		public Dictionary<string, RecoveryTopic> Topics { get; set; } = new Dictionary<string, RecoveryTopic>();
	}
}
