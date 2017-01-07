using System.Collections.Generic;

namespace Server.Storage
{
	public interface QueuesStorage
	{
		void Allocate(string queueName);
		IEnumerable<string> FindAll();
	}
}
