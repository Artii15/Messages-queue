using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Queues.Create
{
	public class CreateQueueService: Service
	{
		readonly CreatingQueue CreatingQueue;

		public CreateQueueService(CreatingQueue creatingQueue)
		{
			CreatingQueue = creatingQueue;
		}

		public CreateQueueResponse Post(CreateQueue request)
		{
			CreatingQueue.Create(request.QueueName);
			return new CreateQueueResponse();
		}
	}
}