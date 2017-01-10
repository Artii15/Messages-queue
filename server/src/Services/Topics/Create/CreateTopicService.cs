using ServiceStack.ServiceInterface;

namespace Server.Services.Topics.Create
{
	public class CreateTopicService: Service
	{
		public CreateTopicResponse Post(CreateTopic request)
		{
			
			return new CreateTopicResponse();
		}
	}
}
