using System.Data;

namespace Server
{
	public abstract class BasicOperation
	{
		protected readonly IDbConnection DBConnection;
		protected const int TIMEOUT = 30000;

		public BasicOperation()
		{
		}
	}
}
