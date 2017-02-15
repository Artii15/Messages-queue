using ServiceStack.ServiceInterface;
using ServiceStack.OrmLite;
using ServiceStack.Common.Web;
using System.Net;

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
			try
			{
				var queuesAndTopics = GettingQueuesAndTopics.Get(request);
				return new QueuesAndTopicsResponse()
				{
					Queues = queuesAndTopics.Queues,
					Topics = queuesAndTopics.Topics
				};
			}
			catch (BadRequestException)
			{
				return new HttpError(HttpStatusCode.BadRequest, "BadRequest");
			}
		}
	}
}
