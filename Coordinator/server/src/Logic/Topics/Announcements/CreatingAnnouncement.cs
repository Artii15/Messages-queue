using System.Data;
using RestSharp;

namespace Server
{
	public class CreatingAnnouncement
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public CreatingAnnouncement(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Create(CreateAnnouncement request)
		{
		}
	}
}
