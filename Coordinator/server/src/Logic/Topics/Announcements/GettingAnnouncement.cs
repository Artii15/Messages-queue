using System.Data;
using RestSharp;

namespace Server
{
	public class GettingAnnouncement : BasicGettingOperation
	{
		public GettingAnnouncement(IDbConnection dbConnection) : base(dbConnection)
		{
		}

		public Announcement Get(GetAnnouncement request, int subscriberId)
		{
			var requestToSend = new RestRequest($"/topics/{request.TopicName}/announcements", Method.GET);
			requestToSend.AddParameter("SubscriberId", subscriberId);

			if (!TopicsQueries.TopicExists(DBConnection, request.TopicName))
				throw new TopicNotExistsException();
			else
			{
				var topic = TopicsQueries.getTopicByName(DBConnection, request.TopicName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, topic.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, topic.Cooperator);

				return PropageteRequestToWorkers<Announcement>(requestToSend, topic, worker, coworker);
			}
		}

		protected override void swapWorkers(ICollection collection)
		{
			TopicsQueries.swapWorkers(DBConnection, (Topic)collection);
		}
	}
}
