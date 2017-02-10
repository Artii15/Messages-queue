using System.Collections.Concurrent;
namespace Server.Logic
{
	public class Locks
	{
		readonly ConcurrentDictionary<string, object> QueuesLocks = new ConcurrentDictionary<string, object>();
		readonly ConcurrentDictionary<string, object> TopicsLocks = new ConcurrentDictionary<string, object>();

		public object TakeQueueLock(string queueName)
		{
			return QueuesLocks.GetOrAdd(queueName, new object());
		}

		public object TakeTopicLock(string topicName)
		{
			return TopicsLocks.GetOrAdd(topicName, new object());
		}

		public void RemoveQueueLock(string queueName)
		{
			object removed;
			QueuesLocks.TryRemove(queueName, out removed);
		}
	}
}