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

		private void initializeQueueDirs(string queueName)
		{
			Directory.CreateDirectory(Paths.makeQueuePath(queueName));
			Directory.CreateDirectory(Paths.makeMessagesPath(queueName));
			Directory.CreateDirectory(Paths.makeTopicsPath(queueName));
		}

		public bool exists(string queueName)
		{
			return File.Exists(Paths.makeQueuePath(queueName));
		}
	}
}