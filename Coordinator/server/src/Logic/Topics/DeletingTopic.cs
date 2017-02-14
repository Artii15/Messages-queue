using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingTopic : BasicTopicOperation
	{
		public DeletingTopic(IDbConnection dbConnection) : base (dbConnection)
		{
		}

		public void Delete(DeleteTopic request)
		{
			var requestToSend = new RestRequest($"/topics/{request.TopicName}", Method.DELETE);

			if (!TopicsQueries.TopicExists(DBConnection, request.TopicName))
				throw new TopicNotExistsException();
			else
			{
				var topic = TopicsQueries.getTopicByName(DBConnection, request.TopicName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, topic.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, topic.Cooperator);

				TopicsQueries.DeleteTopic(DBConnection, request.TopicName);
				PropageteRequestToWorkers(requestToSend, topic, worker, coworker);
			}
		}
	}
}
