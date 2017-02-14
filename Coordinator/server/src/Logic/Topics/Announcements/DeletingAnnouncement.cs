using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingAnnouncement :BasicTopicOperation
	{
		public DeletingAnnouncement(IDbConnection dbConnection) : base (dbConnection)
		{
		}

		public void Delete(DeleteAnnouncement request, int subscriberId)
		{ 
			var requestToSend = new RestRequest($"/topics/{request.TopicName}/announcements/{request.AnnouncementId}", Method.DELETE);
			requestToSend.AddParameter("SubscriberId", subscriberId);

			processRequest(request.TopicName, requestToSend);
		}
	}
}
