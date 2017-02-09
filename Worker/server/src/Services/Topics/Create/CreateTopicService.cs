using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Topics.Create
{
	public class CreateTopicService: IService
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
