using System;
using System.Data;
using RestSharp;

namespace Server
{
	public class CreatingAnnouncement : BasicTopicOperation
	{
		public CreatingAnnouncement(IDbConnection dbConnection) : base (dbConnection)
		{
		}

		public void Create(CreateAnnouncement request)
		{
			var requestToSend = new RestRequest($"/topics/{request.TopicName}/announcements", Method.POST);
			requestToSend.AddParameter("Content", request.Content);

			processRequest(request.TopicName, requestToSend);
		}
	}
}
