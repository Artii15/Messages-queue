using System;
using System.Data;
using RestSharp;

namespace Server
{
	public class GettingMessage
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public GettingMessage(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public Message Get(GetMessage request)
		{
			if (!QueuesQueries.QueueExists(DBConnection, request.QueueName))
				throw new QueueNotExistsException();
			else
			{
				Console.WriteLine("hej");
				var queue = QueuesQueries.getQueueByName(DBConnection, request.QueueName);
				var worker = WorkerQueries.GetWorkerById(DBConnection, queue.Worker);
				var coworker = WorkerQueries.GetWorkerById(DBConnection, queue.Cooperator);

				var client = new RestClient("http://" + worker.Address);
				client.Timeout = TIMEOUT;
				var requestToSend = new RestRequest($"queues/{request.QueueName}/messages", Method.GET);
				requestToSend.AddParameter("Cooperator", coworker.Address);
				Console.WriteLine("hej2");
				var response = client.Execute<Message>(requestToSend);
				Console.WriteLine("hej3");
				if (response.ResponseStatus == ResponseStatus.TimedOut ||
					response.ResponseStatus == ResponseStatus.Error)
				{
					var coworkerClient = new RestClient("http://" + coworker.Address);
					var coworkerRequestToSend = new RestRequest($"queues/{request.QueueName}/messages", Method.GET);
					response = coworkerClient.Execute<Message>(coworkerRequestToSend);
				}

				return response.Data;
			}
		}
	}
}
