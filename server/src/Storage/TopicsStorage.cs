namespace Server.Storage
{
	public interface TopicsStorage
	{
		void Create(string queueName, string topicName);
	}
}
