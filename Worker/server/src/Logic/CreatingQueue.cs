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
		Locks Locks;
		Propagators Propagators;

		public CreatingQueue(Connections connections, Locks locks, Propagators propagators)
		{
			Connections = connections;
			Locks = locks;
			Propagators = propagators;
		}

		public void Create(CreateQueue request)
		{
			var queueLock = Locks.TakeQueueLock(request.Name);
			lock(queueLock)
			{
				if (Locks.QueuesRecoveryLocks.ContainsKey(request.Name))
				{
					throw new Exception($"Queue {request.Name} is inconsistent");
				}

				CreateQueueFile(request.Name);
				if (!string.IsNullOrEmpty(request.Cooperator))
				{
					Propagators.ScheduleQueueOperation(request.Name, () => PropagateRequestToCo(request));
				}
			}
		}

		void CreateQueueFile(string queueName)
		{
			using (var queueDbConn = Connections.ConnectToQueue(queueName))
			{
				queueDbConn.CreateTableIfNotExists<QueueMessage>();
			}
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