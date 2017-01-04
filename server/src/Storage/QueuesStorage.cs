using System.Collections.Generic;
using Server.Entities;

namespace Server.Storage
{
	public interface QueuesStorage
	{
		bool exists(string queueName);
		void allocate(string queueName);
		IEnumerable<Queue> findAll();
	}
}
