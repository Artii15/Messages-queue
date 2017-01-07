using System.Collections.Generic;
using System.IO;

namespace Server.Storage.Files
{
	public class FileQueuesStorage : QueuesStorage
	{
		Paths Paths;

		public FileQueuesStorage(Paths paths)
		{
			Paths = paths;
		}
		
		public void Allocate(string queueName)
		{
			Directory.CreateDirectory(Paths.GetTopicsPath(queueName));
			Directory.CreateDirectory(Paths.GetMessagesPath(queueName));

			Directory.CreateDirectory(Paths.GetQueueMessagesPointersDir(queueName));
			var firstPointerPath = Paths.GetQueueMessagesPointerFile(queueName, QueuePointersNames.First);
			File.Create(firstPointerPath);
			var lastPointerPath = Paths.GetQueueMessagesPointerFile(queueName, QueuePointersNames.Last);
			File.Create(lastPointerPath);
		}

		public IEnumerable<string> FindAll()
		{
			return Directory.EnumerateDirectories(Paths.GetQueuesPath());
		}
	}
}