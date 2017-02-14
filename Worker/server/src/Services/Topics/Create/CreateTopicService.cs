using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Topics.Create
{
	public class CreateTopicService: Service
	{
		readonly CreatingTopic CreatingTopic;

		public CreateTopicService(CreatingTopic creatingTopic)
		{
			CreatingTopic = creatingTopic;
		}

		public CreateTopicResponse Post(CreateTopic request)
		{
			CreatingTopic.Create(request);
			return new CreateTopicResponse();
		}
	}
}
