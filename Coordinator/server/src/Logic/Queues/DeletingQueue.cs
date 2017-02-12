using System.Data;
using RestSharp;
using System;

namespace Server
{
	public class DeletingQueue
	{
		readonly IDbConnection DBConnection;
		const int TIMEOUT = 30000;

		public DeletingQueue(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public void Delete(CreateQueue request)
		{ 
		}
	}
}
