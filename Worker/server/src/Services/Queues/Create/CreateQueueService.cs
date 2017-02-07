using ServiceStack.ServiceHost;
using Server.Logic;

namespace Server.Services.Queues.Create
{
	public class CreateQueueService: IService
	{
		CreatingQueue CreatingQueue;

		public CreateQueueService(CreatingQueue creatingQueue)
		{
			CreatingQueue = creatingQueue;
		}

		public CreateQueueResponse Post(CreateQueue request)
		{
			CreatingQueue.Create(request);
			return new CreateQueueResponse();
		}
	}
}
