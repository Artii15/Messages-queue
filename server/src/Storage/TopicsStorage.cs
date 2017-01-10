namespace Server.Storage
{
	public interface TopicsStorage
	{
		void Create(string queueName, string topicName);

		void Publish(string queueName, string topicName, string message);
	}
}
