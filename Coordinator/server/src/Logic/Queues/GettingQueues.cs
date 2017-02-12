using System;
using System.Collections.Generic;
using System.Data;

namespace Server
{
	public class GettingQueues
	{
		readonly IDbConnection DBConnection;

		public GettingQueues(IDbConnection dbConnection)
		{
			DBConnection = dbConnection;
		}

		public IList<String> Get()
		{
			return QueuesQueries.getQueuesNames(DBConnection);
		}
	}
}
