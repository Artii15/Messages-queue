using System.Data;
using RestSharp;

namespace Server
{
	public class GettingAnnouncement
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public GettingAnnouncement(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public Announcement Get(GetAnnouncement request, int subscriberId)
		{
		}
	}
}
