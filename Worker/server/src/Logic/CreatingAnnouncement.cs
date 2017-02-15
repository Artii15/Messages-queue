using System.Threading;
using ServiceStack.OrmLite;
using Server.Services.Announcements.Create;
using Server.Entities;
using System;
using RestSharp;
using System.Data;

namespace Server.Logic
{
	public class CreatingAnnouncement
	{
		readonly Connections Connections;
		readonly Locks Locks;
		readonly Propagators Propagators;

		public CreatingAnnouncement(Connections connections, Locks locks, Propagators propagators)
		{
			Connections = connections;
			Locks = locks;
			Propagators = propagators;
		}

		public void Create(CreateAnnouncement request)
		{
			var topicLock = Locks.TakeTopicLock(request.TopicName);
			lock (topicLock)
			{
				if (Locks.TopicsRecoveryLocks.ContainsKey(request.TopicName))
				{
					throw new Exception($"Topic {request.TopicName} is inconsistent");
				}

				using (var connection = Connections.ConnectToInitializedTopic(request.TopicName))
				{
					Create(connection, request);
					Monitor.PulseAll(topicLock);
				}
			}
		}

		void Create(IDbConnection connection, CreateAnnouncement request)
		{
			var announcement = new Announcement 
			{ 
				Content = request.Content, 
				CreationTime = request.CreationTime.HasValue ? DateTime.FromBinary(request.CreationTime.Value) : DateTime.UtcNow
			};
			connection.Insert(announcement);
			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				request.CreationTime = announcement.CreationTime.ToBinary();
				Propagators.ScheduleTopicOperation(request.TopicName, () => PropagateToCo(request));
			}
		}

		void PropagateToCo(CreateAnnouncement request)
		{
			var client = new RestClient(request.Cooperator);

			var requestToSend = new RestRequest($"topics/{request.TopicName}/announcements", Method.POST);
			requestToSend.AddParameter("Content", request.Content);
			requestToSend.AddParameter("CreationTime", request.CreationTime);

			client.Execute(requestToSend);
		}
	}
}