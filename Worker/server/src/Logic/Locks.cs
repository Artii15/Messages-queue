using System.Collections.Concurrent;
using System.Threading;

namespace Server.Logic
{
	public class Locks
	{
		public readonly ConcurrentDictionary<string, ManualResetEventSlim> QueuesRecoveryLocks = new ConcurrentDictionary<string, ManualResetEventSlim>();
		public readonly ConcurrentDictionary<string, ManualResetEventSlim> TopicsRecoveryLocks = new ConcurrentDictionary<string, ManualResetEventSlim>();

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

		public void RemoveTopicLock(string topicName)
		{
			object removed;
			TopicsLocks.TryRemove(topicName, out removed);
		}
	}
}