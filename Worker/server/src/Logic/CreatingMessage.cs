using Server.Entities;
using Server.Services.Messages.Create;
using ServiceStack.OrmLite;
using System.Threading;
using RestSharp;

namespace Server.Logic
{
	public class CreatingMessage
	{
		readonly Connections Connections;
		readonly Locks Locks;

		public CreatingMessage(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Create(CreateMessage request)
		{
			var connection = Connections.ConnectToInitializedQueue(request.QueueName);
			var queueLock = Locks.TakeQueueLock(request.QueueName);

			Monitor.Enter(queueLock);

			connection.Insert(new QueueMessage
			{
				Content = request.Content,
				Readed = false
			});

			Monitor.PulseAll(queueLock);
			Monitor.Exit(queueLock);

			connection.Close();
			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				PropagateRequest(request);
			}
		}

		void PropagateRequest(CreateMessage request)
		{
			var client = new RestClient(request.Cooperator);

			var requestToSend = new RestRequest($"queues/{request.QueueName}/messages", Method.POST);
			requestToSend.AddParameter("Content", request.Content);

			client.Execute(requestToSend);
		}
	}
}
