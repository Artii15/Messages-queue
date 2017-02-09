using System.Data;
using RestSharp;
using System;

namespace Server
{
	public class CreatingQueue
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public CreatingQueue(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Create(CreateQueue request)
		{
			if (QueuesQueries.QueueExists(DBConnection, request.Name))
				throw new QueueAlreadyExistsException();
			else
			{
				Worker worker, coworker;
				CalculateQueueWorkers(request.Name, out worker, out coworker);
				QueuesQueries.CreateQueue(DBConnection, request.Name, worker.Id, coworker.Id);
				var client = new RestClient("http://" + worker.Address);
				client.Timeout = TIMEOUT;
				var requestToSend = new RestRequest("queues", Method.POST);
				requestToSend.AddParameter("Name", request.Name);
				requestToSend.AddParameter("Cooperator", coworker.Address);
				var response = client.Execute(requestToSend);
				if (response.ResponseStatus == ResponseStatus.TimedOut || 
				    response.ResponseStatus == ResponseStatus.Error)
				{
					var coworkerClient = new RestClient("http://" + coworker.Address);
					var coworkerRequestToSend = new RestRequest("queues", Method.POST);
					coworkerRequestToSend.AddParameter("Name", request.Name);
					coworkerClient.Execute(requestToSend);
				}
			}
		}

		void CalculateQueueWorkers(string queueName, out Worker worker, out Worker coworker)
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
