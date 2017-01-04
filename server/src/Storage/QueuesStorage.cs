using System.Collections.Generic;

namespace Server.Storage
{
	public interface QueuesStorage
	{
		bool exists(string queueName);
		void allocate(string queueName);
		IEnumerable<string> findAll();
	}
}
