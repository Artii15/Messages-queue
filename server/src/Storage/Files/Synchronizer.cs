using System.Threading;
namespace Server.Storage.Files
{
	public class Synchronizer
	{
		public static EventWaitHandle generateQueueSynchronizationObject(string queueName)
		{
			return new EventWaitHandle(true, EventResetMode.AutoReset, $"MQ_QL_{queueName}");
		}
	}
}
