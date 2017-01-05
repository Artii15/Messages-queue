namespace Server.Storage.Files
{
	public class Paths
	{
		FileStorageConfig Config;

		public Paths(FileStorageConfig config)
		{
			Config = config;
		}

		public string GetQueueMessagesPointerFile(string queueName, string pointerName)
		{
			return $"{GetQueuePath(queueName)}/pointers/{pointerName}";
		}

		public string GetLockFilePath(string queueName)
		{
			return $"{GetQueuePath(queueName)}/lockfile";
		}

		public string GetTopicsPath(string queueName)
		{
			return $"{GetQueuePath(queueName)}/topics";
		}

		public string GetMessagesPath(string queueName)
		{
			return $"{GetQueuePath(queueName)}/messages";
		}

		public string GetQueuePath(string queueName)
		{
			return $"{GetQueuesPath()}/{queueName}";
		}

		public string GetQueuesPath()
		{
			return $"{Config.RootPath}/queues";
		}
	}
}
