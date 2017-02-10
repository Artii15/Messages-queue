using ServiceStack.OrmLite;
using System.Threading;
using Server.Entities;
using Server.Services.Announcements.Get;
using Server.Queries;
using System;
using System.Data;

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
			var topicSubscriber = connection.FirstOrDefault<Subscriber>(subscriber => 
			                                                            subscriber.Id == request.SubscriberId);
			if (topicSubscriber == null)
			{
				connection.Close();
				throw new ArgumentException("Unknown subscriber");
			}

			var announcement = ReadFromDb(connection, topicSubscriber, request);
			connection.Close();
			return announcement;
		}

		Announcement ReadFromDb(IDbConnection connection, Subscriber subscriber, ReadAnnouncement request)
		{
			var topicLock = Locks.TakeTopicLock(request.TopicName);

			Monitor.Enter(topicLock);

			var query = NextAnnouncement.make(connection, subscriber);
			var announcement = connection.FirstOrDefault(query);
			while (announcement == null)
			{
				Monitor.Wait(topicLock);
				announcement = connection.FirstOrDefault(query);
			}

			Monitor.Exit(topicLock);

			return announcement;
		}
	}
}
