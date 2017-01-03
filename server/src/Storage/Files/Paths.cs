namespace Server.Storage.Files
{
	public class Paths
	{
		FileStorageConfig Config;

		public Paths(FileStorageConfig config)
		{
			Config = config;
		}

		public string makeQueuePath(string queueName)
		{
			return $"{getQueuesPath()}/{queueName}";
		}

		public string getQueuesPath()
		{
			return $"{Config.RootPath}/queues";
		}
	}
}
