using ServiceStack.ServiceHost;

namespace Server.Services.CreatingQueue
{
	public class CreateQueueService: IService
	{
		public CreateQueueResponse Post(CreateQueue request)
		{
			return new CreateQueueResponse();
		}
	}
}
