using System.Threading;
using RestSharp;
using Server.Entities;
using Server.Services.Queues.Create;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class CreatingQueue
	{
		Connections Connections;
		Locks Locks;

		public CreatingQueue(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Create(CreateQueue request)
		{
			var queueLock = Locks.TakeQueueLock(request.Name);
			Monitor.Enter(queueLock);
			CreateQueueFile(request.Name);
			Monitor.Exit(queueLock);

			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				PropagateRequestToCo(request);
			}
		}

		void CreateQueueFile(string queueName)
		{
			var queueDbConn = Connections.ConnectToQueue(queueName);
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