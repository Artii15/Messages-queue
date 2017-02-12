using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingAnnouncement
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public DeletingAnnouncement(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Delete(DeleteAnnouncement request)
		{ 
		}
	}
}
