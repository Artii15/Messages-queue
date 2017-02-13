using System.Collections.Generic;
using System.Data;
using System.Linq;
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

		public static Dictionary<string, RecoveryTopic> GetWorkerTopics(IDbConnection dbConnection, long workerId)
		{
			var tList = dbConnection.Select<RecoveryTopic>("Select Name, Address as Cooperator " +
														   "from Topic t join Worker w on t.Cooperator = w.Id " +
														   $"where t.Worker = {workerId} or t.Cooperator = {workerId}");
			return tList.ToDictionary(x => x.Name, x => x);
		}
	}
}
