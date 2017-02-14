using System.Data;
using System.Net;
using RestSharp;

namespace Server
{
	public class BasicTopicOperation : BasicOperation
	{
		public BasicTopicOperation(IDbConnection dbConnection) : base(dbConnection)
		{
		}

		protected void processRequest(string topicName, RestRequest request)
		{
			if (!TopicsQueries.TopicExists(DBConnection, topicName))
				throw new TopicNotExistsException();
			else
			{
				var topic = TopicsQueries.getTopicByName(DBConnection, topicName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, topic.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, topic.Cooperator);

				PropageteRequestToWorkers(request, topic, worker, coworker);
			}
		}

		protected void PropageteRequestToWorkers(RestRequest request, Topic topic, Worker worker, Worker coworker)
		{
			var requestForMaster = request.AddParameter("Cooperator", coworker.Address);
			IRestResponse response;

			if (WorkerQueries.IsWorkerAlive(DBConnection, worker.Id))
			{
				response = PropagateRequest(requestForMaster, worker);
				if (response.ResponseStatus == ResponseStatus.TimedOut ||
					response.ResponseStatus == ResponseStatus.Error)
				{
					response = PropagateRequest(request, coworker);
					TopicsQueries.swapWorkers(DBConnection, topic);
				}
			}
			else
			{
				response = PropagateRequest(request, coworker);
				TopicsQueries.swapWorkers(DBConnection, topic);
			}
			if (response.StatusCode != HttpStatusCode.OK)
				throw new BadRequestException();
		}

		IRestResponse PropagateRequest(IRestRequest request, Worker worker)
		{
			var client = new RestClient(worker.Address);
			client.Timeout = TIMEOUT;
			return client.Execute(request);
		}
	}
}
