using ServiceStack.ServiceInterface;

namespace Server.Services.Queues.Create
{
	public class CreateQueueService: Service
	{
		public CreateQueueResponse Post(CreateQueue request)
		{
			
			return new CreateQueueResponse();
		}
	}
}

