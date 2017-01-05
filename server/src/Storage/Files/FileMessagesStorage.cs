using System.IO;
using Server.Entities;

namespace Server.Storage.Files
{
	public class FileMessagesStorage
	{
		Paths Paths;

		public FileMessagesStorage(Paths paths)
		{
			Paths = paths;
		}

		public void Create(string queueName, Message message)
		{
			InitializeQueueIfNeeded(queueName);
			var dirSynchronizationHandle = Synchronizer.generateQueueSynchronizationObject(queueName);

			dirSynchronizationHandle.WaitOne();
			StoreMessage(queueName, message);
			dirSynchronizationHandle.Set();
		}

		void InitializeQueueIfNeeded(string queueName)
		{
			Directory.CreateDirectory(Paths.GetMessagesPath(queueName));
			Directory.CreateDirectory(Paths.GetQueueMessagesPointersDir(queueName));
		}

		void StoreMessage(string queueName, Message message)
		{

		}
	}
}