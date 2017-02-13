using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Databases.Queues
{
	public class QueueDatabaseService: Service
	{
		readonly DatabaseRecovery DatabaseRecovery;

		public QueueDatabaseService(DatabaseRecovery databaseRecovery)
		{
			DatabaseRecovery = databaseRecovery;
		}

		public QueueDatabaseResponse Put(QueueDatabase request)
		{
			DatabaseRecovery.RecoverQueue(Paths.GetLastSegment(Request.PathInfo), request.RequestStream);
			return new QueueDatabaseResponse();
		}
	}
}
