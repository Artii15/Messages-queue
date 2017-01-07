using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Server.Storage.Files
{
	public class FileMessagesStorage: MessagesStorage
	{
		Paths Paths;
		ConcurrentDictionary<string, ReaderWriterLockSlim> MessagesLocks;
		BinaryFormatter Formatter = new BinaryFormatter();

		public FileMessagesStorage(Paths paths, 
		                           ConcurrentDictionary<string, ReaderWriterLockSlim> messagesLocks)
		{
			Paths = paths;
			MessagesLocks = messagesLocks;
		}

		public void Create(string queueName, string message)
		{
			InitializeQueueIfNeeded(queueName);
			ReaderWriterLockSlim messagesLock;

			if (MessagesLocks.TryGetValue(queueName, out messagesLock))
			{
				messagesLock.EnterWriteLock();
				StoreMessage(queueName, message);
				messagesLock.ExitReadLock();
			}
			else
			{
				throw new Exception(); //TODO Throw more specific exception
			}
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

		void StoreMessage(string queueName, string message)
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

		string SaveMessageToFile(string queueName, string message)
		{
			var messageId = Guid.NewGuid().ToString();
			var storedMessage = new StoredMessage { Content = message, Next = null };
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
				lastMessageStream.SetLength(0);
				lastMessageStream.Seek(0, SeekOrigin.Begin);
				Formatter.Serialize(lastMessageStream, lastMessage);
			}
		}

		public string TryToReadNextMessage(string queueName)
		{
			ReaderWriterLockSlim messagesLock;
			if (MessagesLocks.TryGetValue(queueName, out messagesLock))
			{
				messagesLock.EnterWriteLock();
				string message = readNextMessage(queueName);
				messagesLock.ExitWriteLock();
				return message;
			}
			throw new Exception(); // TODO More specific exception
		}

		string readNextMessage(string queueName)
		{
			var messagePointerPath = Paths.GetQueueMessagesPointerFile(queueName, QueuePointersNames.First);
			var nextMessageId = File.ReadAllText(messagePointerPath);

			string message = null;
			if (nextMessageId != "")
			{
				message = File.ReadAllText(Paths.GetMessagePath(queueName, nextMessageId));
			}
			return message;
		}
	}
}