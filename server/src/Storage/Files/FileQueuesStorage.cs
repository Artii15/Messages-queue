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
		
		public void allocate(string queueName)
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
			Directory.CreateDirectory(Paths.MakeMessagesPath(queueName));
			Directory.CreateDirectory(Paths.MakeTopicsPath(queueName));
		}

		public bool exists(string queueName)
		{
			return File.Exists(Paths.MakeQueuePath(queueName));
		}

		public IEnumerable<string> findAll()
		{
			return Directory.EnumerateDirectories(Paths.GetQueuesPath());
		}
	}
}