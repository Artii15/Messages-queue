using Server.Storage;

namespace Server.Logic
{
	public class CreatingQueue
	{
		readonly QueuesStorage QueuesStorage;

		public CreatingQueue(QueuesStorage queuesStorage)
		{
			QueuesStorage = queuesStorage;
		}

		public void Create(string queueName)
		{
			QueuesStorage.Create(queueName);
		}
	}
}
