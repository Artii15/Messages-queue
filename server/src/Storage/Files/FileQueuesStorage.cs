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

		public FileQueuesStorage(Paths paths,
		                        ConcurrentDictionary<string, ReaderWriterLockSlim> messagesLocks)
		{
			Paths = paths;
			MessagesLocks = messagesLocks;
		}
		
		public void Allocate(string queueName)
		{
			createQueueFiles(queueName);
			MessagesLocks.TryAdd(queueName, new ReaderWriterLockSlim());
		}

		void createQueueFiles(string queueName)
		{
			Directory.CreateDirectory(Paths.GetTopicsPath(queueName));
			Directory.CreateDirectory(Paths.GetMessagesPath(queueName));

			Directory.CreateDirectory(Paths.GetQueueMessagesPointersDir(queueName));
			var firstPointerPath = Paths.GetQueueMessagesPointerFile(queueName, QueuePointersNames.First);
			File.Create(firstPointerPath).Close();
			var lastPointerPath = Paths.GetQueueMessagesPointerFile(queueName, QueuePointersNames.Last);
			File.Create(lastPointerPath).Close();
		}

		public IEnumerable<string> FindAll()
		{
			return from directoryPath in Directory.GetDirectories(Paths.GetQueuesPath())
				   select directoryPath.Substring(directoryPath.LastIndexOf('/') + 1);
		}
	}
}