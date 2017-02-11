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

		public CreatingAnnouncement(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Create(CreateAnnouncement request)
		{
			var topicLock = Locks.TakeTopicLock(request.TopicName);
			lock (topicLock)
			{
				using (var connection = Connections.ConnectToInitializedTopic(request.TopicName))
				{
					Create(connection, request);
					Monitor.PulseAll(topicLock);
				}
			}
		}

		void Create(IDbConnection connection, CreateAnnouncement request)
		{
			var announcement = new Announcement { Content = request.Content, CreationTime = DateTime.UtcNow };
			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				request.CreationTime = announcement.CreationTime;
				PropagateToCo(request);
			}
			connection.Insert(announcement);
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