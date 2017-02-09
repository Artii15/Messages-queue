using System;
using System.Data;
using Server.Entities;
using ServiceStack.OrmLite;

namespace Server.Queries
{
	public static class NextAnnouncementByDate
	{
		public static SqlExpressionVisitor<Announcement> make(IDbConnection connection, DateTime date)
		{
			return connection.CreateExpression<Announcement>()
				             .Where(announcement => announcement.CreationTime >= date)
				             .OrderByDescending(announcement => announcement.CreationTime);
		}
	}
}
