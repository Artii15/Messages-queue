using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Queues.Delete
{
	public class DeleteQueueService: Service
	{
		public DeletingQueue DeletingQueue;

		public DeleteQueueService(DeletingQueue deletingQueue)
		{
			DeletingQueue = deletingQueue;
		}

		public DeleteQueueResponse Delete(DeleteQueue request)
		{
			DeletingQueue.Delete(request);
			return new DeleteQueueResponse();
		}
	}
}
