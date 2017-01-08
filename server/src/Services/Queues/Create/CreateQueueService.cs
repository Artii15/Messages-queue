using System;
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
			if (string.IsNullOrWhiteSpace(request.QueueName))
			{
				throw new ArgumentException("QueueName must not be empty");
			}
			CreatingQueue.Create(request.QueueName);
			return new CreateQueueResponse();
		}
	}
}