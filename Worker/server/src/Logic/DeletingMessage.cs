using System.Data;
using Server.Queries;
using ServiceStack.OrmLite;
using System;
using Server.Services.Messages.Delete;
using Server.Entities;
using RestSharp;

namespace Server.Logic
{
	public class DeletingMessage
	{
		readonly Connections Connections;
		Locks Locks;

		public DeletingMessage(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Delete(DeleteMessage request)
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
					if (IsDeleteDecisionImposedByCooperator(request))
					{
						ForceDelete(connection, request);
					}
					else
					{
						TryDelete(connection, request);
					}
				}
			}
		}

		bool IsDeleteDecisionImposedByCooperator(DeleteMessage request)
		{
			return string.IsNullOrEmpty(request.Cooperator);
		}

		void ForceDelete(IDbConnection connection, DeleteMessage request)
		{
			connection.UpdateOnly(new QueueMessage { Readed = true },
			                      message => new { message.Readed },
			                      message => message.Id == request.MessageId);
		}

		void TryDelete(IDbConnection connection, DeleteMessage request)
		{
			var firstMessageInQueue = connection.FirstOrDefault(FirstMessageQuery.make(connection));
			if (firstMessageInQueue == null || firstMessageInQueue.Id != request.MessageId)
			{
				throw new ArgumentException("Provided message was not first in queue");
			}
			PropagateRequest(request);
			firstMessageInQueue.Readed = true;
			connection.Update(firstMessageInQueue);
		}

		void PropagateRequest(DeleteMessage request)
		{
			var client = new RestClient(request.Cooperator);

			var requestToSend = new RestRequest($"queues/{request.QueueName}/messages/{request.MessageId}", Method.DELETE);

			client.Execute(requestToSend);
		}
	}
}