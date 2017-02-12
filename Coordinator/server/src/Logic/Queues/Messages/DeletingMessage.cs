using System.Data;
using RestSharp;

namespace Server
{
	public class DeletingMessage
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public DeletingMessage(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Delete(DeleteMessage request)
		{
		}
	}
}
