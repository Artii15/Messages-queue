using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingTopic
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public DeletingTopic(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Delete(DeleteTopic request)
		{
			if (!TopicsQueries.TopicExists(DBConnection, request.TopicName))
				throw new TopicNotExistsException();
			else
			{
				var topic = TopicsQueries.getTopicByName(DBConnection, request.TopicName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, topic.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, topic.Cooperator);

				TopicsQueries.DeleteTopic(DBConnection, request.TopicName);
				if (WorkerQueries.IsWorkerAlive(DBConnection, worker.Id))
				{
					var response = PropagateRequest(request, worker, coworker);
					if (response.ResponseStatus == ResponseStatus.TimedOut ||
						response.ResponseStatus == ResponseStatus.Error)
					{
						PropagateRequestToCoworker(request, coworker);
						TopicsQueries.swapWorkers(DBConnection, topic);
					}
				}
				else
				{
					PropagateRequestToCoworker(request, coworker);
					TopicsQueries.swapWorkers(DBConnection, topic);
				}
			}
		}

		IRestResponse PropagateRequest(DeleteTopic request, Worker worker, Worker coworker)
		{
			var client = new RestClient(worker.Address);
			client.Timeout = TIMEOUT;
			var requestToSend = new RestRequest($"/topics/{request.TopicName}", Method.DELETE);
			requestToSend.AddParameter("Cooperator", coworker.Address);
			return client.Execute(requestToSend);
		}

		void PropagateRequestToCoworker(DeleteTopic request, Worker coworker)
		{
			var coworkerClient = new RestClient(coworker.Address);
			var coworkerRequestToSend = new RestRequest($"/topics/{request.TopicName}", Method.DELETE);
			coworkerClient.Execute(coworkerRequestToSend);
		}
	}
}
