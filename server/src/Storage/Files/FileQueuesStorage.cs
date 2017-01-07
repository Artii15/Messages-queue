using System.Collections.Generic;
using System.IO;
using Server.Storage.Exceptions;

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
			if (!exists(queueName))
			{
				initializeQueueDirs(queueName);
			}
			else
			{
				throw new QueueAlreadyExists();
			}
		}

		void initializeQueueDirs(string queueName)
		{
			Directory.CreateDirectory(Paths.GetMessagesPath(queueName));
			Directory.CreateDirectory(Paths.GetTopicsPath(queueName));
		}

		public bool exists(string queueName)
		{
			return File.Exists(Paths.GetQueuePath(queueName));
		}

		public IEnumerable<string> FindAll()
		{
			return Directory.EnumerateDirectories(Paths.GetQueuesPath());
		}
	}
}