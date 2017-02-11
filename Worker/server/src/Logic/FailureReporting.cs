using System;
using System.IO;
using System.Threading.Tasks;
using RestSharp;
using Server.Services.Failures.Queues;
using Server.Services.Failures.Topics;

namespace Server.Logic
{
	public class FailureReporting
	{
		Locks Locks;

		public FailureReporting(Locks locks)
		{
			Locks = locks;
		}

		public void Report(QueueFailure request)
		{
			HandleFailure(new FailureDescriptor 
			{ 
				PathToDbFile = Connections.PathToDbFile(Connections.QueuesDir, request.Name),
				DbLock = Locks.TakeQueueLock(request.Name),
				CooperatorAddress = request.Cooperator,
				RecoveryCategory = "queues",
				DbName = request.Name
			});
		}

		public void Report(TopicFailure request)
		{
			HandleFailure(new FailureDescriptor
			{
				PathToDbFile = Connections.PathToDbFile(Connections.TopicsDir, request.Name),
				DbLock = Locks.TakeTopicLock(request.Name),
				CooperatorAddress = request.Cooperator,
				RecoveryCategory = "topics",
				DbName = request.Name
			});
		}

		void HandleFailure(FailureDescriptor failureDescriptor)
		{
			if (!File.Exists(failureDescriptor.PathToDbFile))
			{
				throw new ArgumentException();
			}
			Task.Factory.StartNew(() => 
			{
				lock (failureDescriptor.DbLock)
				{
					var client = new RestClient(failureDescriptor.CooperatorAddress);
					var request = new RestRequest($"databases/{failureDescriptor.RecoveryCategory}/{failureDescriptor.DbName}", Method.PUT);
					request.RequestFormat = DataFormat.Json;
					request.AddHeader("Content-Type", "multipart/form-data");
					request.AddFile("DatabaseFile", Path.GetFullPath(failureDescriptor.PathToDbFile));
					client.Execute(request);
				}
			});
		}
	}

	struct FailureDescriptor
	{
		public string PathToDbFile { get; set; }
		public object DbLock { get; set; }
		public string CooperatorAddress { get; set; }
		public string RecoveryCategory { get; set; }
		public string DbName { get; set; }
	}
}
