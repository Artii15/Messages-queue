namespace Server.Storage.Files
{
	public class Paths
	{
		FileStorageConfig Config;

		public Paths(FileStorageConfig config)
		{
			Config = config;
		}

		public string MakeTopicsPath(string queueName)
		{
			return $"{MakeQueuePath(queueName)}/topics";
		}

		public string MakeMessagesPath(string queueName)
		{
			return $"{MakeQueuePath(queueName)}/messages";
		}

		public string MakeQueuePath(string queueName)
		{
			return $"{GetQueuesPath()}/{queueName}";
		}

		public string GetQueuesPath()
		{
			return $"{Config.RootPath}/queues";
		}
	}
}
