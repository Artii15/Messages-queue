using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace Server.Storage.Files
{
	public class FileTopicsStorage: TopicsStorage
	{
		readonly Paths Paths;
		readonly ConcurrentDictionary<string, ReaderWriterLockSlim> TopicsLocks;
		readonly ConcurrentDictionary<string, object> TopicsMonitors;

		public FileTopicsStorage(Paths paths,
		                         ConcurrentDictionary<string, ReaderWriterLockSlim> topicsLocks,
		                         ConcurrentDictionary<string, object> topicsMonitors)
		{
			Paths = paths;
			TopicsLocks = topicsLocks;
			TopicsMonitors = topicsMonitors;
		}

		public void Create(string queueName, string topicName)
		{
			CreateTopicFiles(queueName, topicName);
			var topicSyncKey = MakeTopicSyncKey(queueName, topicName);
			TopicsLocks.TryAdd(topicSyncKey, new ReaderWriterLockSlim());
			TopicsMonitors.TryAdd(topicSyncKey, new object());
		}

		void CreateTopicFiles(string queueName, string topicName)
		{
			Directory.CreateDirectory(Paths.GetTopicPath(queueName, topicName));
			Directory.CreateDirectory(Paths.GetTopicMessages(queueName, topicName));
			Directory.CreateDirectory(Paths.GetTopicPointers(queueName, topicName));
			File.Create(Paths.GetTopicPointer(queueName, topicName, PointersNames.First)).Close();
			File.Create(Paths.GetTopicPointer(queueName, topicName, PointersNames.Last)).Close();
		}

		public void Publish(string queueName, string topicName, string message)
		{
			var topicSyncKey = MakeTopicSyncKey(queueName, topicName);
			var topicLock = TopicsLocks[topicSyncKey];
			var topicMonitor = TopicsMonitors[topicSyncKey];

			topicLock.EnterWriteLock();

			WriteMessage(queueName, topicName, message);
			Monitor.Enter(topicMonitor);
			Monitor.PulseAll(topicMonitor);
			Monitor.Exit(topicMonitor);

			topicLock.ExitWriteLock();
		}

		string MakeTopicSyncKey(string queueName, string topicName)
		{
			return $"{queueName}-{topicName}";
		}

		void WriteMessage(string queueName, string topicName, string message)
		{
			var fileStorage = new FileStorage(Paths.GetTopicMessages(queueName, topicName),
											  Paths.GetTopicPointer(queueName, topicName, PointersNames.First),
											  Paths.GetTopicPointer(queueName, topicName, PointersNames.Last));
			fileStorage.AddMessage(message);
		}
	}
}
