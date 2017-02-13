using System.Data;
using RestSharp;
using System;

namespace Server
{
	public class CreatingTopic
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public CreatingTopic(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Create(CreateTopic request)
		{
			if (TopicsQueries.TopicExists(DBConnection, request.Name))
				throw new TopicAlreadyExistsException();
			else
			{
				Worker worker, coworker;
				CalculateTopicWorkers(request.Name, out worker, out coworker);
				TopicsQueries.CreateTopic(DBConnection, request.Name, worker.Id, coworker.Id);

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

		void CalculateTopicWorkers(string queueName, out Worker worker, out Worker coworker)
		{
			var workerCount = WorkerQueries.GetWorkersCount(DBConnection);
			var nameHash = Math.Abs(queueName.GetHashCode());
			var workerPosition = nameHash % workerCount;
			var coworkerPosition = (nameHash + 1) % workerCount;
			worker = WorkerQueries.GetWorker(DBConnection, workerPosition);
			coworker = WorkerQueries.GetWorker(DBConnection, coworkerPosition);
		}

		IRestResponse PropagateRequest(CreateTopic request, Worker worker, Worker coworker)
		{
			var client = new RestClient(worker.Address);
			client.Timeout = TIMEOUT;
			var requestToSend = new RestRequest("topic", Method.POST);
			requestToSend.AddParameter("Name", request.Name);
			requestToSend.AddParameter("Cooperator", coworker.Address);
			return client.Execute(requestToSend);
		}

		void PropagateRequestToCoworker(CreateTopic request, Worker coworker)
		{
			var coworkerClient = new RestClient(coworker.Address);
			var coworkerRequestToSend = new RestRequest("topic", Method.POST);
			coworkerRequestToSend.AddParameter("Name", request.Name);
			coworkerClient.Execute(coworkerRequestToSend);
		}
	}
}
