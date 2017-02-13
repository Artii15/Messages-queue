using System;
using System.Data;
using RestSharp;

namespace Server
{
	public class CreatingAnnouncement
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public CreatingAnnouncement(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Create(CreateAnnouncement request)
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
					var response = PropagateRequest(request, worker, coworker);
					if (response.ResponseStatus == ResponseStatus.TimedOut ||
						response.ResponseStatus == ResponseStatus.Error)
						PropagateRequestToCoworker(request, coworker);
				}
				else
					PropagateRequestToCoworker(request, coworker);
			}
		}

		IRestResponse PropagateRequest(CreateAnnouncement request, Worker worker, Worker coworker)
		{
			var client = new RestClient($"http://{worker.Address}");
			client.Timeout = TIMEOUT;
			var requestToSend = new RestRequest($"/topics/{request.TopicName}/announcements", Method.POST);
			requestToSend.AddParameter("Content", request.Content);
			requestToSend.AddParameter("CreationTime", DateTime.UtcNow);
			requestToSend.AddParameter("Cooperator", coworker.Address);
			return client.Execute(requestToSend);
		}

		void PropagateRequestToCoworker(CreateAnnouncement request, Worker coworker)
		{
			var coworkerClient = new RestClient($"http://{coworker.Address}");
			var coworkerRequestToSend = new RestRequest($"/topics/{request.TopicName}/announcements", Method.POST);
			coworkerRequestToSend.AddParameter("Content", request.Content);
			coworkerRequestToSend.AddParameter("CreationTime", DateTime.UtcNow);
			coworkerClient.Execute(coworkerRequestToSend);
		}
	}
}
