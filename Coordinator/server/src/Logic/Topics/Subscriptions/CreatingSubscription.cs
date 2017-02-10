using System.Data;
using RestSharp;
using System;

namespace Server
{
	public class CreatingSubscription
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public CreatingSubscription(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Create(CreateSubscription request)
		{
		}
	}
}
