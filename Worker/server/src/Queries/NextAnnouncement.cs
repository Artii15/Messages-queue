using System.Data;
using Server.Entities;
using ServiceStack.OrmLite;

namespace Server.Queries
{
	public static class NextAnnouncement
	{
		public static SqlExpressionVisitor<Announcement> make(IDbConnection connection, Subscriber subscriber)
		{
			return (subscriber.LastAnnouncementId.HasValue)
				? NextAnnouncementById.make(connection, subscriber.LastAnnouncementId.Value)
									  : NextAnnouncementByDate.make(connection, subscriber.CreationTime.ToUniversalTime());
		}
	}
}