using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingAnnouncement
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public DeletingAnnouncement(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Delete(DeleteAnnouncement request, int subscriberId)
		{ 
			if (!TopicsQueries.TopicExists(DBConnection, request.TopicName))
				throw new TopicNotExistsException();
			else
			{
				var topic = TopicsQueries.getTopicByName(DBConnection, request.TopicName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, topic.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, topic.Cooperator);

				if (WorkerQueries.IsWorkerAlive(DBConnection, worker.Id))
				{
					var response = PropagateRequest(request, subscriberId, worker, coworker);
					if (response.ResponseStatus == ResponseStatus.TimedOut ||
						response.ResponseStatus == ResponseStatus.Error)
					{
						PropagateRequestToCoworker(request, subscriberId, coworker);
						TopicsQueries.swapWorkers(DBConnection, topic);
					}
				}
				else
				{
					PropagateRequestToCoworker(request, subscriberId, coworker);
					TopicsQueries.swapWorkers(DBConnection, topic);
				}
			}
		}

		IRestResponse PropagateRequest(DeleteAnnouncement request, int subscriberId, Worker worker, Worker coworker)
		{
			var client = new RestClient($"http://{worker.Address}");
			client.Timeout = TIMEOUT;
			var requestToSend = new RestRequest($"/topics/{request.TopicName}/announcements/{request.AnnouncementId}", Method.DELETE);
			requestToSend.AddParameter("SubscriberId", subscriberId);
			requestToSend.AddParameter("Cooperator", coworker.Address);
			return client.Execute(requestToSend);
		}

		void PropagateRequestToCoworker(DeleteAnnouncement request, int subscriberId, Worker coworker)
		{
			var coworkerClient = new RestClient($"http://{coworker.Address}");
			var coworkerRequestToSend = new RestRequest($"/topics/{request.TopicName}/announcements/{request.AnnouncementId}", Method.DELETE);
			coworkerRequestToSend.AddParameter("SubscriberId", subscriberId);
			coworkerClient.Execute(coworkerRequestToSend);
		}
	}
}
