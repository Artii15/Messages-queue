using System;
using System.Data;
using Server.Entities;
using ServiceStack.OrmLite;

namespace Server.Queries
{
	public static class NextAnnouncementByDate
	{
		public static SqlExpressionVisitor<Announcement> make(IDbConnection connection, DateTime subscriptionDate)
		{
			return connection.CreateExpression<Announcement>()
				             .Where(announcement => announcement.CreationTime >= subscriptionDate)
				             .OrderBy(announcement => announcement.CreationTime);
		}
	}
}
