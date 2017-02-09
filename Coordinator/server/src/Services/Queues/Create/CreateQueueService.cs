using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;

namespace Server
{
	[Authenticate]
	public class CreateQueueService : Service
	{
		readonly CreatingQueue CreatingQueue;

		public CreateQueueService()
		{
			CreatingQueue = new CreatingQueue(Db);
			Db.CreateTableIfNotExists<Queue>();
		}

		public CreateQueueResponse Post(CreateQueue request)
		{
			CreatingQueue.Create(request);
			return new CreateQueueResponse();
		}
	}
}
