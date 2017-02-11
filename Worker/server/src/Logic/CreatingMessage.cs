using Server.Entities;
using Server.Services.Messages.Create;
using ServiceStack.OrmLite;
using System.Threading;
using RestSharp;
using System;
using System.Data;

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
			var queueLock = Locks.TakeQueueLock(request.QueueName);
			lock (queueLock)
			{
				if (Locks.QueuesRecoveryLocks.ContainsKey(request.QueueName))
				{
					throw new Exception($"Queue {request.QueueName} is inconsistent");
				}

				using (var connection = Connections.ConnectToInitializedQueue(request.QueueName))
				{
					Create(connection, request);		
					Monitor.PulseAll(queueLock);
				}
			}
		}

		void Create(IDbConnection connection, CreateMessage request)
		{
			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				PropagateRequest(request);
			}

			connection.Insert(new QueueMessage
			{
				Content = request.Content,
				Readed = false
			});
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
