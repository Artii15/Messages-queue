using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Server.Entities;

namespace Server.Storage.Files
{
	public class FileMessagesStorage
	{
		Paths Paths;
		BinaryFormatter Formatter = new BinaryFormatter();

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
			var messageId = SaveMessageToFile(queueName, message);

			var queueFirstPointerPath = Paths.GetQueueMessagesPointerFile(queueName, QueuePointersNames.First);
			var queueLastPointerPath = Paths.GetQueueMessagesPointerFile(queueName, QueuePointersNames.Last);
			var lastMessageId = File.ReadAllText(queueLastPointerPath);
			if (lastMessageId == "")
			{
				File.WriteAllText(queueFirstPointerPath, messageId);
			}
			else
			{
				LetLastMessagePointToNewMessage(Paths.GetMessagePath(queueName, lastMessageId), messageId);
			}
			File.WriteAllText(queueLastPointerPath, messageId);
		}

		string SaveMessageToFile(string queueName, Message message)
		{
			var messageId = Guid.NewGuid().ToString();
			var storedMessage = new StoredMessage { Message = message, Next = null };
			using (var newMessageFileStream = new FileStream(Paths.GetMessagePath(queueName, messageId), FileMode.Create))
			{
				Formatter.Serialize(newMessageFileStream, storedMessage);
			}
			return messageId;
		}

		void LetLastMessagePointToNewMessage(string lastMessagePath, string newMessageId)
		{
			using (var lastMessageStream = new FileStream(lastMessagePath, FileMode.Open))
			{
				var lastMessage = (StoredMessage)Formatter.Deserialize(lastMessageStream);
				lastMessage.Next = newMessageId;
				lastMessageStream.Position = 0;
				Formatter.Serialize(lastMessageStream, lastMessage);
			}
		}
	}
}