using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Queues.Delete
{
	public class DeleteQueueService: IService
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
