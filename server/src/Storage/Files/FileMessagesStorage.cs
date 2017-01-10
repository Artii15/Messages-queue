﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Server.Entities;

namespace Server.Storage.Files
{
	public class FileMessagesStorage: MessagesStorage
	{
		readonly Paths Paths;
		ConcurrentDictionary<string, ReaderWriterLockSlim> MessagesLocks;
		readonly ConcurrentDictionary<string, ManualResetEventSlim> MessagesEvents;
		readonly BinaryFormatter Formatter = new BinaryFormatter();

		public FileMessagesStorage(Paths paths, 
		                           ConcurrentDictionary<string, ReaderWriterLockSlim> messagesLocks,
		                           ConcurrentDictionary<string, ManualResetEventSlim> messagesEvents)
		{
			Paths = paths;
			MessagesLocks = messagesLocks;
			MessagesEvents = messagesEvents;
		}

		public void Create(string queueName, string message)
		{
			ReaderWriterLockSlim messagesLock;

			if (MessagesLocks.TryGetValue(queueName, out messagesLock))
			{
				messagesLock.EnterWriteLock();
				StoreMessage(queueName, message);
				MessagesEvents[queueName].Set();
				messagesLock.ExitWriteLock();
			}
			else
			{
				throw new Exception(); //TODO Throw more specific exception
			}
		}

		void StoreMessage(string queueName, string message)
		{
			var fileStorage = new FileStorage(Paths.GetMessagesPath(queueName),
			                                  Paths.GetPointerFile(queueName, PointersNames.First),
			                                  Paths.GetPointerFile(queueName, PointersNames.Last));
			fileStorage.AddMessage(message);
		}

		public Message ReadNextMessage(string queueName)
		{
			ReaderWriterLockSlim messagesLock = MessagesLocks[queueName];
			ManualResetEventSlim messagesEvent = MessagesEvents[queueName];

			Message? message = null;
			while (message == null)
			{
				messagesEvent.Wait();
				messagesLock.EnterReadLock();
				message = readNextMessage(queueName);
				messagesLock.ExitReadLock();
			}

			return (Message)message;
		}

		Message? readNextMessage(string queueName)
		{
			var messagePointerPath = Paths.GetPointerFile(queueName, PointersNames.First);
			var nextMessageId = File.ReadAllText(messagePointerPath);

			Message? message = null;
			if (nextMessageId != "")
			{
				var storedMessage = DeserializeMessage(queueName, nextMessageId);
				message = new Message(nextMessageId, storedMessage.Content);
			}
			return message;
		}

		StoredMessage DeserializeMessage(string queueName, string messageId)
		{
			var messagePath = Paths.GetMessagePath(queueName, messageId);
			using (var messageStream = new FileStream(messagePath, FileMode.Open))
			{
				return (StoredMessage)Formatter.Deserialize(messageStream);
			}
		}

		public MessageRemovingStatus TryToPop(string queueName, string messageId)
		{
			ReaderWriterLockSlim messagesLock;
			if (MessagesLocks.TryGetValue(queueName, out messagesLock))
			{
				messagesLock.EnterWriteLock();
				var removingStatus = RemoveIfPossible(queueName, messageId);
				messagesLock.ExitWriteLock();

				return removingStatus;
			}
			throw new ArgumentException();
		}

		MessageRemovingStatus RemoveIfPossible(string queueName, string messageId)
		{
			var firstMessageId = File.ReadAllText(Paths.GetPointerFile(queueName, PointersNames.First));
			if (firstMessageId == messageId)
			{
				var messageToRemove = DeserializeMessage(queueName, messageId);
				File.WriteAllText(Paths.GetPointerFile(queueName, PointersNames.First), messageToRemove.Next);

				var lastPointerPath = Paths.GetPointerFile(queueName, PointersNames.Last);
				if (File.ReadAllText(lastPointerPath) == messageId)
				{
					File.WriteAllText(lastPointerPath, messageToRemove.Next);
					MessagesEvents[queueName].Reset();
				}
				return new MessageRemovingStatus(true, messageToRemove.Next);
			}
			return new MessageRemovingStatus(false, firstMessageId);
		}
	}
}