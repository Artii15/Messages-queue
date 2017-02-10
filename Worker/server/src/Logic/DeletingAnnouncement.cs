using System;
using Server.Entities;
using Server.Queries;
using Server.Services.Announcements.Delete;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class DeletingAnnouncement
	{
		readonly Connections Connections;

		public DeletingAnnouncement(Connections connections)
		{
			Connections = connections;
		}

		public void Delete(DeleteAnnouncement request)
		{
			using (var connection = Connections.ConnectToInitializedTopic(request.TopicName))
			{
				var subscriber = connection.GetById<Subscriber>(request.SubscriberId);
				var announcement = connection.First(NextAnnouncement.make(connection, subscriber));

				if (announcement.Id != request.AnnouncementId)
				{
					throw new ArgumentException("Invalid announcement");
				}

				connection.UpdateOnly(new Subscriber { LastAnnouncementId = announcement.Id },
				                      subscription => new { subscription.LastAnnouncementId },
				                      subscription => subscription.Id == subscriber.Id);
			}
		}
	}
}
