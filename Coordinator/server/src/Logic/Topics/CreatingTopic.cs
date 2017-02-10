using System.Data;
using RestSharp;
using System;

namespace Server
{
	public class CreatingTopic
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public CreatingTopic(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Create(CreateTopic request)
		{
		}
	}
}
