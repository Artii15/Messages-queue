using System.Data;
using System.Threading;
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
			var connection = Connections.ConnectToInitializedQueue(request.QueueName);
			var queueLock = Locks.TakeQueueLock(request.QueueName);

			Monitor.Enter(queueLock);

			var onDeleteCallback = IsDeleteDecisionImposedByCooperator(request)
				? ForceDelete(connection, request)
				: TryDelete(connection, request);

			Monitor.Exit(queueLock);
			connection.Close();

			onDeleteCallback();
		}

		bool IsDeleteDecisionImposedByCooperator(DeleteMessage request)
		{
			return string.IsNullOrEmpty(request.Cooperator);
		}

		Action ForceDelete(IDbConnection connection, DeleteMessage request)
		{
			connection.UpdateOnly(new QueueMessage { Readed = true },
			                      message => new { message.Readed },
			                      message => message.Id == request.MessageId);
			return () => { };
		}

		Action TryDelete(IDbConnection connection, DeleteMessage request)
		{
			var firstMessageInQueue = connection.FirstOrDefault(FirstMessageQuery.make(connection));
			if (firstMessageInQueue != null && firstMessageInQueue.Id == request.MessageId)
			{
				firstMessageInQueue.Readed = true;
				connection.Update(firstMessageInQueue);
				return () => PropagateRequest(request);
			}
			return () => { throw new ArgumentException("Provided message was not first in queue"); };
		}

		void PropagateRequest(DeleteMessage request)
		{
			var client = new RestClient(request.Cooperator);

			var requestToSend = new RestRequest($"queues/{request.QueueName}/messages/{request.MessageId}", Method.DELETE);

			client.Execute(requestToSend);
		}
	}
}