using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RestSharp;
using Server.Entities;

namespace Server
{
	public class RecoveryController
	{
		QueuesAndTopics QueuesAndTopicsToRecover;

		public void BeginRecovery(QueuesAndTopics queuesAndTopicsToRecover)
		{
			QueuesAndTopicsToRecover = queuesAndTopicsToRecover;
		}

		void RecoverQueues()
		{
			foreach (var queue in QueuesAndTopicsToRecover.Queues)
			{
				var recoveryTask = RecoverMessagesContainer(queue.Value, "queues");
				recoveryTask.ContinueWith(_ => { });
			}
		}

		void RecoverTopics()
		{
			foreach (var topic in QueuesAndTopicsToRecover.Topics)
			{
				RecoverMessagesContainer(topic.Value, "topics", response =>
				{

				});
			}
		}

		Task RecoverMessagesContainer(MessagesContainer container, string category)
		{
			var dbFilePath = $"{category}/{container.GetName()}.sqlite";
			File.Delete(dbFilePath);
			return Task.Factory.StartNew(() =>
			{
				using (var dbFile = File.OpenWrite(dbFilePath))
				{
					var worker = new RestClient(container.GetCooperator());
					var request = new RestRequest($"databases/{category}/{container.GetName()}", Method.GET);
					request.ResponseWriter = db => db.CopyTo(dbFile);
					worker.DownloadData(request);
				}
			});
		}
	}
}