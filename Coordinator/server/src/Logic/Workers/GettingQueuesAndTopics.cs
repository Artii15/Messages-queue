using System.Data;

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
			if (Encrypt.EncryptToken(request.Time, request.WorkerId, request.Address, request.Token))
			{
				var queues = QueuesQueries.GetWorkerQueues(DBConnection, request.WorkerId);
				var topics = TopicsQueries.GetWorkerTopics(DBConnection, request.WorkerId);
				return new QueuesAndTopics
				{
					Queues = queues,
					Topics = topics
				};
			}
			throw new BadRequestException();
		}
	}
}
