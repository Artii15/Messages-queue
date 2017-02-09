using System.Data;
using Server.Entities;
using ServiceStack.OrmLite;

namespace Server.Queries
{
	public static class FirstMessageQuery
	{
		public static SqlExpressionVisitor<QueueMessage> make(IDbConnection connection)
		{
			return connection.CreateExpression<QueueMessage>()
				             .Where(message => !message.Readed)
				             .OrderBy(message => message.Id);
		}
	}
}