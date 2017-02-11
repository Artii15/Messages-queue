using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Server.Entities;
using Server.Logic;

namespace Server
{
	public class RecoveryController
	{
		QueuesAndTopics QueuesAndTopicsToRecover;
		Locks Locks;

		public RecoveryController(Locks locks)
		{
			Locks = locks;
		}

		public void BeginRecovery(QueuesAndTopics queuesAndTopicsToRecover)
		{
			QueuesAndTopicsToRecover = queuesAndTopicsToRecover;
		}

		void RecoverQueues()
		{
			foreach (var queue in QueuesAndTopicsToRecover.Queues)
			{
				var queueToRecover = queue.Value;
				RecoverMessagesContainer(queueToRecover, "queues").ContinueWith(_ => Recover(queueToRecover, Locks.QueuesRecoveryLocks));
			}
		}

		void RecoverTopics()
		{
			foreach (var topic in QueuesAndTopicsToRecover.Topics)
			{
				var topicToRecover = topic.Value;
				RecoverMessagesContainer(topicToRecover, "topics").ContinueWith(_ => Recover(topicToRecover, Locks.TopicsRecoveryLocks));
			}
		}

		void Recover(MessagesContainer container, ConcurrentDictionary<string, ManualResetEventSlim> recoveryLocks)
		{
			ManualResetEventSlim recoveryLock;
			if (recoveryLocks.TryRemove(container.GetName(), out recoveryLock))
			{
				recoveryLock.Set();
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