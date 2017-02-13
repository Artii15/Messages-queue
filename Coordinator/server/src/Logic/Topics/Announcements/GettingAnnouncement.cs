using System.Data;
using RestSharp;

namespace Server
{
	public class GettingAnnouncement
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public GettingAnnouncement(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public Announcement Get(GetAnnouncement request, int subscriberId)
		{
			if (!TopicsQueries.TopicExists(DBConnection, request.TopicName))
				throw new TopicNotExistsException();
			else
			{
				var topic = TopicsQueries.getTopicByName(DBConnection, request.TopicName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, topic.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, topic.Cooperator);
				IRestResponse<Announcement> response;

				if (WorkerQueries.IsWorkerAlive(DBConnection, worker.Id))
				{
					response = PropagateRequest(request, subscriberId, worker, coworker);
					if (response.ResponseStatus == ResponseStatus.TimedOut ||
						response.ResponseStatus == ResponseStatus.Error)
					{
						if (!WorkerQueries.IsWorkerAlive(DBConnection, worker.Id))
						{
							response = PropagateRequestToCoworker(request, subscriberId, coworker);
							TopicsQueries.swapWorkers(DBConnection, topic);
						}
						else
							throw new NoNewContentToGetException();
					}
				}
				else
				{
					response = PropagateRequestToCoworker(request, subscriberId, coworker);
					TopicsQueries.swapWorkers(DBConnection, topic);
				}

				return response.Data;
			}
		}

		IRestResponse<Announcement> PropagateRequest(GetAnnouncement request, int subscriberId, Worker worker, Worker coworker)
		{
			var client = new RestClient(worker.Address);
			client.Timeout = TIMEOUT;
			var requestToSend = new RestRequest($"/topics/{request.TopicName}/announcements", Method.GET);
			requestToSend.AddParameter("SubscriberId", subscriberId);
			return client.Execute<Announcement>(requestToSend);
		}

		IRestResponse<Announcement> PropagateRequestToCoworker(GetAnnouncement request, int subscriberId, Worker coworker)
		{
			var coworkerClient = new RestClient(coworker.Address);
			var coworkerRequestToSend = new RestRequest($"/topics/{request.TopicName}/announcements", Method.GET);
			coworkerRequestToSend.AddParameter("SubscriberId", subscriberId);
			return coworkerClient.Execute<Announcement>(coworkerRequestToSend);
		}
	}
}
