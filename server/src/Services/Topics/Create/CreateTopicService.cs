using Server.Storage;
using ServiceStack.ServiceInterface;

namespace Server.Services.Topics.Create
{
	public class CreateTopicService: Service
	{
		TopicsStorage TopicsStorage;

		public CreateTopicService(TopicsStorage topicsStorage)
		{
			TopicsStorage = topicsStorage;
		}

		public CreateTopicResponse Post(CreateTopic request)
		{
			TopicsStorage.Create(request.QueueName, request.TopicName);
			return new CreateTopicResponse();
		}
	}
}
