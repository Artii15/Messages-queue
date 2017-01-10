using Server.Storage;

namespace Server.Logic
{
	public class CreatingTopic
	{
		readonly TopicsStorage TopicsStorage;

		public CreatingTopic(TopicsStorage topicsStorage)
		{
			TopicsStorage = topicsStorage;
		}

		public void Create(string queueName, string topicName)
		{
			TopicsStorage.Create(queueName, topicName);
		}
	}
}