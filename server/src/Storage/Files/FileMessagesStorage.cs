using System;
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

			var firstPointerPath = Paths.GetQueueMessagesPointerFile(queueName, QueuePointersNames.First);
			if (!File.Exists(firstPointerPath)) File.Create(firstPointerPath);

			var lastPointerPath = Paths.GetQueueMessagesPointerFile(queueName, QueuePointersNames.Last);
			if (!File.Exists(lastPointerPath)) File.Create(lastPointerPath);
		}

		void StoreMessage(string queueName, Message message)
		{
			var messageId = Guid.NewGuid().ToString();
			var newMessageFileStream = new FileStream(Paths.GetMessagePath(queueName, messageId), FileMode.Create);
			//var storedMessage = new StoredMessage(

			var queueTailPointerPath = Paths.GetQueueMessagesPointerFile(queueName, QueuePointersNames.LastMessagePointerName);
			using (var queueTailFile = new BinaryReader(new FileStream(queueTailPointerPath, FileMode.OpenOrCreate)))
			{
				
			}
		}
	}
}