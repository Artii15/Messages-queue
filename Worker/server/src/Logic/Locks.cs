using System.Collections.Concurrent;
namespace Server.Logic
{
	public class Locks
	{
		readonly ConcurrentDictionary<string, object> locks = new ConcurrentDictionary<string, object>();

		public object TakeQueueLock(string queueName)
		{
			return locks.GetOrAdd(queueName, new object());
		}
	}
}
