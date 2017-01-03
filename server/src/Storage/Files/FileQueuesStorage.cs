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
				Directory.CreateDirectory(Paths.makeQueuePath(queueName));
			}
			else
			{
				throw new QueueAlreadyExists();
			}
		}

		public bool exists(string queueName)
		{
			return File.Exists(Paths.makeQueuePath(queueName));
		}
	}
}