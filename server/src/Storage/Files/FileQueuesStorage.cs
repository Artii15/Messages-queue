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
		
		public void allocate(string queueName)
		{
			
		}

		public bool exists(string queueName)
		{
			return File.Exists(Paths.makeQueuePath(queueName));
		}
	}
}