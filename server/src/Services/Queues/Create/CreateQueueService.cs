using Server.Storage;
using ServiceStack.ServiceInterface;

namespace Server.Services.Queues.Create
{
	public class CreateQueueService: Service
	{
		private QueuesStorage QueuesStorage;

		public CreateQueueService(QueuesStorage queuesStorage)
		{
			QueuesStorage = queuesStorage;
		}

		public CreateQueueResponse Post(CreateQueue request)
		{
			QueuesStorage.allocate(request.QueueName);
			return new CreateQueueResponse();
		}
	}
}