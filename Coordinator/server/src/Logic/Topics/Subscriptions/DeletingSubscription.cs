using System.Data;
using RestSharp;
using System;

namespace Server
{
	public class DeletingSubscription
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public DeletingSubscription(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Delete(DeleteSubscription request, int subscriberId)
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
						PropagateRequestToCoworker(request, subscriberId, coworker);
				}
				else
					PropagateRequestToCoworker(request, subscriberId, coworker);
			}
		}

		IRestResponse PropagateRequest(DeleteSubscription request, int subscriberId, Worker worker, Worker coworker)
		{
			var client = new RestClient($"http://{worker.Address}");
			client.Timeout = TIMEOUT;
			var requestToSend = new RestRequest($"topics/{request.TopicName}/subscriptions/{subscriberId}", Method.DELETE);
			requestToSend.AddParameter("Cooperator", coworker.Address);
			return client.Execute(requestToSend);
		}

		void PropagateRequestToCoworker(DeleteSubscription request, int subscriberId, Worker coworker)
		{
			var coworkerClient = new RestClient($"http://{coworker.Address}");
			var coworkerRequestToSend = new RestRequest($"topics/{request.TopicName}/subscriptions/{subscriberId}", Method.DELETE);
			coworkerClient.Execute(coworkerRequestToSend);
		}
	}
}
