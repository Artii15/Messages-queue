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

		public CreatingTopic(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
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

				if (!string.IsNullOrEmpty(request.Cooperator))
				{
					PropagateRequest(request);
				}

				using (var connection = Connections.ConnectToTopic(request.Name))
				{
					connection.CreateTableIfNotExists<Announcement>();
					connection.CreateTableIfNotExists<Subscriber>();
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
