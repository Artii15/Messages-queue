using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Server.Storage.Files
{
	public class FileTopicsStorage: TopicsStorage
	{
		readonly BinaryFormatter Formatter = new BinaryFormatter();
		readonly Paths Paths;
		readonly ConcurrentDictionary<string, ReaderWriterLockSlim> topicsLocks;
		ConcurrentDictionary<string, object> topicsMonitors;

		public FileTopicsStorage(Paths paths)
		{
			Paths = paths;
		}

		public void Create(string queueName, string topicName)
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
			var topicLock = topicsLocks[topicSyncKey];
			var topicMonitor = topicsMonitors[topicSyncKey];

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
			var storedMessage = new StoredMessage { Content = message, Next = "" };
		}
	}
}
