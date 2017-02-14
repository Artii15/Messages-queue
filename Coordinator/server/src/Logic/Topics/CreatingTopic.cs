using System.Data;
using RestSharp;
using System;

namespace Server
{
	public class CreatingTopic : BasicTopicOperation
	{
		public CreatingTopic(IDbConnection dbConnection) : base (dbConnection)
		{
		}

		public void Create(CreateTopic request)
		{
			var requestToSend = new RestRequest("topics", Method.POST);
			requestToSend.AddParameter("Name", request.Name);

			if (TopicsQueries.TopicExists(DBConnection, request.Name))
				throw new TopicAlreadyExistsException();
			else
			{
				Worker worker, coworker;
				CalculateTopicWorkers(request.Name, out worker, out coworker);
				TopicsQueries.CreateTopic(DBConnection, request.Name, worker.Id, coworker.Id);
				var topic = TopicsQueries.getTopicByName(DBConnection, request.Name);

				PropageteRequestToWorkers(requestToSend, topic, worker, coworker);
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
	}
}
