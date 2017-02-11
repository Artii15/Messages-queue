using System;
using System.Data;
using RestSharp;
using Server.Entities;
using Server.Queries;
using Server.Services.Announcements.Delete;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class DeletingAnnouncement
	{
		readonly Connections Connections;
		readonly Locks Locks;

		public DeletingAnnouncement(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Delete(DeleteAnnouncement request)
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
					Delete(connection, request);
				}
			}
		}

		void Delete(IDbConnection connection, DeleteAnnouncement request)
		{
			var subscriber = connection.GetById<Subscriber>(request.SubscriberId);
			var announcement = connection.First(NextAnnouncement.make(connection, subscriber));

			if (announcement.Id != request.AnnouncementId)
			{
				throw new ArgumentException("Invalid announcement");
			}

			Propagate(request);
			connection.UpdateOnly(new Subscriber { LastAnnouncementId = announcement.Id },
								  subscription => new { subscription.LastAnnouncementId },
								  subscription => subscription.Id == subscriber.Id);
		}

		void Propagate(DeleteAnnouncement request)
		{
			if (!string.IsNullOrEmpty(request.Cooperator))
			{
				var client = new RestClient(request.Cooperator);

				var requestToSend = new RestRequest($"topics/{request.TopicName}/announcements/{request.AnnouncementId}", Method.DELETE);
				requestToSend.AddBody(new { request.SubscriberId });

				client.Execute(requestToSend);
			}
		}
	}
}
