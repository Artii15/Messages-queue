using System.Data;
using Server.Entities;
using ServiceStack.OrmLite;

namespace Server
{
	public static class NextAnnouncementById
	{
		public static SqlExpressionVisitor<Announcement> make(IDbConnection connection, int lastId)
		{
			return connection.CreateExpression<Announcement>()
				             .Where(announcement => announcement.Id > lastId)
							 .OrderBy(announcement => announcement.CreationTime);
		}
	}
}
