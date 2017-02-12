using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Server.Logic
{
	public class Propagators
	{
		ConcurrentDictionary<string, Task> QueuesPropagators = new ConcurrentDictionary<string, Task>();
		ConcurrentDictionary<string, Task> TopicsPropagators = new ConcurrentDictionary<string, Task>();

		public void ScheduleQueueOperation(string queueName, Action operation)
		{
			var previousTask = QueuesPropagators.GetOrAdd(queueName, Task.Factory.StartNew(() => {}));
			QueuesPropagators[queueName] = ScheduleOperation(previousTask, operation);
		}

		public void ScheduleTopicOperation(string topicName, Action operation)
		{
			var previousTask = QueuesPropagators.GetOrAdd(topicName, Task.Factory.StartNew(() => {}));
			TopicsPropagators[topicName] = ScheduleOperation(previousTask, operation);
		}

		Task ScheduleOperation(Task previousTask, Action operation)
		{
			return previousTask.ContinueWith(_ => operation);
		}
	}
}
