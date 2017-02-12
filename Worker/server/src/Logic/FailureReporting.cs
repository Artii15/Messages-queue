using System;
using System.IO;
using RestSharp;
using Server.Services.Failures.Queues;
using Server.Services.Failures.Topics;

namespace Server.Logic
{
	public class FailureReporting
	{
		Locks Locks;
		Propagators Propagators;

		public FailureReporting(Locks locks, Propagators propagators)
		{
			Locks = locks;
			Propagators = propagators;
		}

		public void Report(QueueFailure request)
		{
			var recoveryAction = HandleFailure(new FailureDescriptor 
			{ 
				PathToDbFile = Connections.PathToDbFile(Connections.QueuesDir, request.Name),
				DbLock = Locks.TakeQueueLock(request.Name),
				CooperatorAddress = request.Cooperator,
				RecoveryCategory = "queues",
				DbName = request.Name
			});
			Propagators.ScheduleQueueOperation(request.Name, recoveryAction);
		}

		public void Report(TopicFailure request)
		{
			var recoveryAction = HandleFailure(new FailureDescriptor
			{
				PathToDbFile = Connections.PathToDbFile(Connections.TopicsDir, request.Name),
				DbLock = Locks.TakeTopicLock(request.Name),
				CooperatorAddress = request.Cooperator,
				RecoveryCategory = "topics",
				DbName = request.Name
			});
			Propagators.ScheduleTopicOperation(request.Name, recoveryAction);
		}

		Action HandleFailure(FailureDescriptor failureDescriptor)
		{
			if (!File.Exists(failureDescriptor.PathToDbFile))
			{
				throw new ArgumentException();
			}

			return () => 
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
			};
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
