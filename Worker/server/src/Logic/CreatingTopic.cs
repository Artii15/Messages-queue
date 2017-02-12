using System;
using RestSharp;
using Server.Entities;
using Server.Services.Topics.Create;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class CreatingTopic
	{
		readonly Connections Connections;
		readonly Locks Locks;
		Propagators Propagators;

		public CreatingTopic(Connections connections, Locks locks, Propagators propagators)
		{
			Connections = connections;
			Locks = locks;
			Propagators = propagators;
		}

		public void Create(CreateTopic request)
		{
			var topicLock = Locks.TakeTopicLock(request.Name);
			lock (topicLock)
			{
				if (Locks.TopicsRecoveryLocks.ContainsKey(request.Name))
				{
					throw new Exception($"Topic {request.Name} is inconsistent");
				}

				using (var connection = Connections.ConnectToTopic(request.Name))
				{
					connection.CreateTableIfNotExists<Announcement>();
					connection.CreateTableIfNotExists<Subscriber>();
				}

				if (!string.IsNullOrEmpty(request.Cooperator))
				{
					Propagators.ScheduleTopicOperation(request.Name, () => PropagateRequest(request));
				}
			}
		}

		void PropagateRequest(CreateTopic request)
		{
			var client = new RestClient(request.Cooperator);

			var requestToSend = new RestRequest("topics", Method.POST);
			requestToSend.AddParameter("Name", request.Name);

			client.Execute(requestToSend);
		}
	}
}
