using System.Collections.Generic;

namespace Server.Storage
{
	public interface QueuesStorage
	{
		void Create(string queueName);
		IEnumerable<string> FindAll();
		bool HasMessages(string queueName);
	}
}
