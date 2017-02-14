using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Databases.Topics
{
	public class TopicDatabaseService: Service
	{
		readonly DatabaseRecovery DatabaseRecovery;

		public TopicDatabaseService(DatabaseRecovery databaseRecovery)
		{
			DatabaseRecovery = databaseRecovery;
		}

		public TopicDatabaseResponse Put(TopicDatabase request)
		{
			DatabaseRecovery.RecoverTopic(Paths.GetLastSegment(Request.PathInfo), request.RequestStream);
			return new TopicDatabaseResponse();
		}
	}
}