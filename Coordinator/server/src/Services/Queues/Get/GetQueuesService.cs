using ServiceStack.ServiceInterface;

namespace Server
{
	[Authenticate]
	public class GetQueuesService : Service
	{
		readonly GettingQueues GettingQueues;

		public GetQueuesService()
		{
			GettingQueues = new GettingQueues(Db);
		}

		public object Get(GetQueues request)
		{
			var queues = GettingQueues.Get();

			return new GetQueuesResponse()
			{
				Queues = queues
			};
		}
	}
}
