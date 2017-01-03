namespace Server.Storage.Files
{
	public class Paths
	{
		FileStorageConfig Config;

		public Paths(FileStorageConfig config)
		{
			Config = config;
		}


		public string makeTopicsPath(string queueName)
		{
			return $"{makeQueuePath(queueName)}/topics";
		}

		public string makeMessagesPath(string queueName)
		{
			return $"{makeQueuePath(queueName)}/messages";
		}

		public string makeQueuePath(string queueName)
		{
			return $"{getQueuesPath()}/{queueName}";
		}

		private string getQueuesPath()
		{
			return $"{Config.RootPath}/queues";
		}
	}
}
