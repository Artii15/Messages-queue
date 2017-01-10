namespace Server.Storage.Files
{
	public class Paths
	{
		readonly FileStorageConfig Config;

		public Paths(FileStorageConfig config)
		{
			Config = config;
		}

		public string GetPointerFile(string queueName, string pointerName)
		{
			return $"{GetQueueMessagesPointersDir(queueName)}/{pointerName}";
		}

		public string GetQueueMessagesPointersDir(string queueName)
		{
			return $"{GetQueuePath(queueName)}/pointers";
		}

		public string GetTopicMessages(string queueName, string topicName)
		{
			return $"{GetTopicPath(queueName, topicName)}/messages";
		}

		public string GetTopicPointer(string queueName, string topicName, string pointerName)
		{
			return $"{GetTopicPointers(queueName, topicName)}/{pointerName}";
		}

		public string GetTopicPointers(string queueName, string topicName)
		{
			return $"{GetTopicPath(queueName, topicName)}/pointers";
		}

		public string GetTopicPath(string queueName, string topicName)
		{
			return $"{GetTopicsPath(queueName)}/{topicName}";
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
