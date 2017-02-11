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
		readonly Connections Connections;
		Locks Locks;

		public FailureReporting(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void ReportQueueFailure(QueueFailure request)
		{
			HandleFailure(new FailureDescriptor 
			{ 
				PathToDbFile = Connections.PathToDbFile(Connections.QueuesDir, request.Name),
				DbLock = Locks.TakeQueueLock(request.Name),
				CooperatorAddress = request.Cooperator,
				RecoveryCategory = "queues"
			});
		}

		public void ReportTopicFailure(TopicFailure request)
		{
			HandleFailure(new FailureDescriptor
			{
				PathToDbFile = Connections.PathToDbFile(Connections.TopicsDir, request.Name),
				DbLock = Locks.TakeTopicLock(request.Name),
				CooperatorAddress = request.Cooperator,
				RecoveryCategory = "topics"
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
					var request = new RestRequest($"recoveries/{failureDescriptor.RecoveryCategory}", Method.POST);
					request.RequestFormat = DataFormat.Json;
					request.AddHeader("Content-Type", "multipart/form-data");
					request.AddFile("database", Path.GetFullPath(failureDescriptor.PathToDbFile));
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
	}
}
