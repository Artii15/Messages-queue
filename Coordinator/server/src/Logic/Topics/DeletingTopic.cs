using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingTopic
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public DeletingTopic(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Delete(DeleteTopic request)
		{
		}
	}
}
