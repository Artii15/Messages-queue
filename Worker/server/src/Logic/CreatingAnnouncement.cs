using System.Threading;
using ServiceStack.OrmLite;
using Server.Services.Announcements.Create;
using Server.Entities;
using System;
using RestSharp;

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
			var connection = Connections.ConnectToInitializedTopic(request.TopicName);
			var topicLock = Locks.TakeTopicLock(request.TopicName);

			Monitor.Enter(topicLock);

			var announcement = new Announcement { Content = request.Content, CreationTime = DateTime.UtcNow };
			connection.Insert(announcement);

			Monitor.PulseAll(topicLock);
			Monitor.Exit(topicLock);

			connection.Close();

			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				request.CreationTime = announcement.CreationTime;
				PropagateToCo(request);
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
