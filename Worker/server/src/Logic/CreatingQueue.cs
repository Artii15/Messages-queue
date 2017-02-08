using System;
using RestSharp;
using Server.Entities;
using Server.Services.Queues.Create;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class CreatingQueue
	{
		Connections Connections;

		public CreatingQueue(Connections connections)
		{
			Connections = connections;
		}

		public void Create(CreateQueue request)
		{
			try
			{
				if (!string.IsNullOrEmpty(request.Cooperator))
				{
					PropagateRequestToCo(request);
				}
				CreateQueueFile(request.Name);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
		}

		void CreateQueueFile(string queueName)
		{
			var queueDbConn = Connections.ConnectToQueue(queueName).Open();
			queueDbConn.CreateTableIfNotExists<QueueMessage>();
			queueDbConn.Close();
		}

		void PropagateRequestToCo(CreateQueue request)
		{
			var client = new RestClient(request.Cooperator);

			var requestToSend = new RestRequest("queues", Method.POST);
			requestToSend.AddParameter("Name", request.Name);

			client.Execute(requestToSend);
		}
	}
}