using System;
using System.Collections.Generic;
using System.Data;

namespace Server
{
	public class GettingTopics
	{
		readonly IDbConnection DBConnection;

		public GettingTopics(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public IList<String> Get()
		{
			return TopicsQueries.getTopicsNames(DBConnection);
		}
	}
}
