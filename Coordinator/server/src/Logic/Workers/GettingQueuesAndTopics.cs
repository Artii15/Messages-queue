using System;
using System.Data;
using ServiceStack.OrmLite;

namespace Server
{
	public class GettingQueuesAndTopics
	{
		readonly IDbConnection DBConnection;

		public GettingQueuesAndTopics(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public QueuesAndTopics Get(QueuesAndTopicsRequest request)
		{
			if (Encrypt.EncryptToken(request.Time, request.Token))
			{
				var queues = QueuesQueries.GetWorkerQueues(DBConnection, request.WorkerId);
				var topics = TopicsQueries.GetWorkerTopics(DBConnection, request.WorkerId);
				return new QueuesAndTopics()
				{
					Queues = queues,
					Topics = topics
				};
			}
			else
				throw new BadRequestException();
		}
	}
}
