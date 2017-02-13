using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Databases.Queues
{
	public class QueueDatabaseService: IService
	{
		readonly DatabaseRecovery DatabaseRecovery;

		public QueueDatabaseService(DatabaseRecovery databaseRecovery)
		{
			DatabaseRecovery = databaseRecovery;
		}

		public QueueDatabaseResponse Put(QueueDatabase request)
		{
			DatabaseRecovery.Recover(request);
			return new QueueDatabaseResponse();
		}
	}
}
