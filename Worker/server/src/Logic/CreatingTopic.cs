using System.Threading;
using Server.Entities;
using Server.Services.Topics.Create;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class CreatingTopic
	{
		readonly Connections Connections;
		readonly Locks Locks;

		public CreatingTopic(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Create(CreateTopic request)
		{
			var connection = Connections.ConnectToTopic(request.Name);
			var topicLock = Locks.TakeTopicLock(request.Name);

			Monitor.Enter(topicLock);
			connection.CreateTableIfNotExists<Announcement>();
			connection.CreateTableIfNotExists<Subscription>();
			Monitor.Exit(topicLock);
		}
	}
}
