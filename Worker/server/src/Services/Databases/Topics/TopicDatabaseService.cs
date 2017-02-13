using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Databases.Topics
{
	public class TopicDatabaseService: IService
	{
		readonly DatabaseRecovery DatabaseRecovery;

		public TopicDatabaseService(DatabaseRecovery databaseRecovery)
		{
			DatabaseRecovery = databaseRecovery;
		}

		public TopicDatabaseResponse Put(TopicDatabase request)
		{
			DatabaseRecovery.Recover(request);
			return new TopicDatabaseResponse();
		}
	}
}
