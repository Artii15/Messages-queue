using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

namespace Server.Storage.Files
{
	public class FileQueuesStorage : QueuesStorage
	{
		readonly Paths Paths;
		readonly ConcurrentDictionary<string, ReaderWriterLockSlim> MessagesLocks;
		readonly ConcurrentDictionary<string, ManualResetEventSlim> MessagesEvents;

		public FileQueuesStorage(Paths paths,
		                         ConcurrentDictionary<string, ReaderWriterLockSlim> messagesLocks,
		                         ConcurrentDictionary<string, ManualResetEventSlim> messagesEvents)
		{
			Paths = paths;
			MessagesLocks = messagesLocks;
			MessagesEvents = messagesEvents;
		}
		
		public void Create(string queueName)
		{
			createQueueFiles(queueName);
			MessagesLocks.TryAdd(queueName, new ReaderWriterLockSlim());
			MessagesEvents.TryAdd(queueName, new ManualResetEventSlim(false));
		}

		void createQueueFiles(string queueName)
		{
			Directory.CreateDirectory(Paths.GetTopicsPath(queueName));
			Directory.CreateDirectory(Paths.GetMessagesPath(queueName));

			Directory.CreateDirectory(Paths.GetQueueMessagesPointersDir(queueName));
			var firstPointerPath = Paths.GetPointerFile(queueName, PointersNames.First);
			File.Create(firstPointerPath).Close();
			var lastPointerPath = Paths.GetPointerFile(queueName, PointersNames.Last);
			File.Create(lastPointerPath).Close();
		}

		public IEnumerable<string> FindAll()
		{
			return from directoryPath in Directory.GetDirectories(Paths.GetQueuesPath())
				   select directoryPath.Substring(directoryPath.LastIndexOf('/') + 1);
		}

		public bool HasMessages(string queueName)
		{
			var MessagesLock = MessagesLocks[queueName];
			MessagesLock.EnterReadLock();

			var queueHeadPointerPath = Paths.GetPointerFile(queueName, PointersNames.First);
			var hasMessages = !string.IsNullOrWhiteSpace(File.ReadAllText(queueHeadPointerPath)); 

			MessagesLock.ExitReadLock();

			return hasMessages;
		}
	}
}