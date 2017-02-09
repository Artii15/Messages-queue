using System;
using ServiceStack.OrmLite;
using System.Threading;
using Server.Entities;
using Server.Services.Announcements.Get;
using Server.Queries;

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

			var topicSubscriber = connection.FirstOrDefault<Subscriber>(subscriber => 
			                                                            subscriber.Id == request.SubscriberId);
			if (topicSubscriber != null)
			{
				var query = (topicSubscriber.LastAnnouncementId.HasValue) 
					? NextAnnouncementById.make(connection, topicSubscriber.LastAnnouncementId.Value) 
				                          : NextAnnouncementByDate.make(connection, topicSubscriber.CreationTime);
			}

			Monitor.Exit(topicLock);

			connection.Close();
		}
	}
}
