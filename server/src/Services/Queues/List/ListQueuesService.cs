using Server.Storage;
using ServiceStack.ServiceInterface;

namespace Server.Services.Queues.List
{
	public class ListQueuesService: Service
	{
		QueuesStorage QueuesStorage;

		public ListQueuesService(QueuesStorage queuesStorage)
		{
			QueuesStorage = queuesStorage;
		}

		public ListQueuesResponse get(ListQueues request)
		{
			return new ListQueuesResponse { Queues = QueuesStorage.FindAll() };
		}
	}
}
