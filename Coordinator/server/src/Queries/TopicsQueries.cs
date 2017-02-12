using System.Collections.Generic;
using System.Data;
using ServiceStack.OrmLite;

namespace Server
{
	public static class TopicsQueries
	{
		public static bool TopicExists(IDbConnection dbConnection, string name)
		{
			return dbConnection.Select<Topic>(topic => topic.Name == name).Count != 0;
		}

		public static void CreateTopic(IDbConnection dbConnection, string name, long worker, long coworker)
		{
			dbConnection.Insert(new Topic { Name = name, Worker = worker, Cooperator = coworker });
		}

		public static void DeleteTopic(IDbConnection dbConnection, string name)
		{
			dbConnection.Delete<Topic>(topic => topic.Name == name);
		}

		public static Topic getTopicByName(IDbConnection dbConnection, string topicName)
		{
			var exp = dbConnection.CreateExpression<Topic>().Where(topic => topic.Name == topicName);
			return dbConnection.FirstOrDefault(exp);
		}

		public static List<string> getTopicsNames(IDbConnection dbConnection)
		{
			return new List<string>(dbConnection.List<string>("select Name from Topic"));
		}
	}
}
