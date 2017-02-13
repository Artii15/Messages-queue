using System.Data;
using RestSharp;
using System;

namespace Server
{
	public class CreatingSubscription
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public CreatingSubscription(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Create(CreateSubscription request, int subscriberId)
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

		IRestResponse PropagateRequest(CreateSubscription request, int subscriberId, Worker worker, Worker coworker)
		{
			var client = new RestClient(worker.Address);
			client.Timeout = TIMEOUT;
			var requestToSend = new RestRequest($"topics/{request.TopicName}/subscriptions", Method.POST);
			requestToSend.AddParameter("SubscriberId", subscriberId);
			requestToSend.AddParameter("CreationTime", DateTime.UtcNow);
			requestToSend.AddParameter("Cooperator", coworker.Address);
			return client.Execute(requestToSend);
		}

		void PropagateRequestToCoworker(CreateSubscription request, int subscriberId, Worker coworker)
		{
			var coworkerClient = new RestClient(coworker.Address);
			var coworkerRequestToSend = new RestRequest($"topics/{request.TopicName}/subscriptions", Method.POST);
			coworkerRequestToSend.AddParameter("SubscriberId", subscriberId);
			coworkerRequestToSend.AddParameter("CreationTime", DateTime.UtcNow);
			coworkerClient.Execute(coworkerRequestToSend);
		}
	}
}
