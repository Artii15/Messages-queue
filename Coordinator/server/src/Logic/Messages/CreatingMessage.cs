using System.Data;
using RestSharp;
namespace Server
{
	public class CreatingMessage
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public CreatingMessage(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Create(CreateMessage request)
		{ 
		}
	}
}
