using ServiceStack.ServiceInterface;
using ServiceStack.OrmLite;

namespace Server
{
	public class QueuesAndTopicsService : Service
	{
		readonly GettingQueuesAndTopics GettingQueuesAndTopics;

		public QueuesAndTopicsService()
		{
			GettingQueuesAndTopics = new GettingQueuesAndTopics(Db);
			Db.CreateTableIfNotExists<Queue>();
			Db.CreateTableIfNotExists<Topic>();
		}

		public object Get(QueuesAndTopicsRequest request)
		{
			var queuesAndTopics = GettingQueuesAndTopics.Get(request);
			return new QueuesAndTopicsResponse()
			{
				Queues = queuesAndTopics.Queues,
				Topics = queuesAndTopics.Topics
			};
		}
	}
}
