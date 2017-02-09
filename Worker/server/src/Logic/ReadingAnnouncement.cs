using System;
using ServiceStack.OrmLite;
using System.Threading;
using Server.Entities;
using Server.Services.Announcements.Get;

namespace Server.Logic
{
	public class ReadingAnnouncement
	{
		readonly Connections Connections;
		readonly Locks Locks;

		public ReadingAnnouncement(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public Announcement Read(ReadAnnouncement request)
		{
			var connection = Connections.ConnectToInitializedTopic(request.TopicName);
			var topicLock = Locks.TakeTopicLock(request.TopicName);

			Monitor.Enter(topicLock);

			var topicSubscription = connection.FirstOrDefault<Subscription>(
				subscription => subscription.SubscriberId == request.SubscriberId);

			Monitor.Exit(topicLock);

			connection.Close();
		}
	}
}
