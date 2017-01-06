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
			return $"{GetQueueMessagesPointersDir(queueName)}/{pointerName}";
		}

		public string GetQueueMessagesPointersDir(string queueName)
		{
			return $"{GetQueuePath(queueName)}/pointers";
		}

		public string GetTopicsPath(string queueName)
		{
			return $"{GetQueuePath(queueName)}/topics";
		}

		public string GetMessagePath(string queueName, string messageId)
		{
			return $"{GetMessagesPath(queueName)}/{messageId}";
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
