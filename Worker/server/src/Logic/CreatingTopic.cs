using System.Threading;
using Server.Services.Topics.Create;

namespace Server.Logic
{
	public class CreatingTopic
	{
		Connections Connections;
		readonly Locks Locks;

		public CreatingTopic(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Create(CreateTopic request)
		{
			var topicLock = Locks.TakeTopicLock(request.Name);
			Monitor.Enter(topicLock);
			Monitor.Exit(topicLock);
		}
	}
}
