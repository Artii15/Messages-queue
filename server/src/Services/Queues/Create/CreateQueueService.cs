using ServiceStack.ServiceInterface;

namespace Server.Services.Queues.Create
{
	public class CreateQueueService: Service
	{
		public CreateQueueResponse Post(CreateQueue request)
		{
			var queue = new Queue { Name = request.QueueName };
			return new CreateQueueResponse();
		}
	}
}

