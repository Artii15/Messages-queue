using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server.Storage.Files
{
	public class FileStorage
	{
		readonly string MessagesPath;
		readonly string FirstPointer;
		readonly string LastPointer;
		readonly BinaryFormatter Formatter = new BinaryFormatter();

		public FileStorage(string messagesPath, string firstPointer, string lastPointer)
		{
			MessagesPath = messagesPath;
			FirstPointer = firstPointer;
			LastPointer = lastPointer;
		}

		public string AddMessage(string message)
		{
			var newMessageId = SaveMessageToFile(message);

			var lastMessageId = File.ReadAllText(LastPointer);
			if (lastMessageId == "")
			{
				File.WriteAllText(FirstPointer, newMessageId);
			}
			else
			{
				LetLastMessagePointToNewMessage(lastMessageId, newMessageId);
			}
			File.WriteAllText(LastPointer, newMessageId);

			return newMessageId;
		}

		string SaveMessageToFile(string message)
		{
			var messageId = Guid.NewGuid().ToString();
			var storedMessage = new StoredMessage { Content = message, Next = "" };
			using (var newMessageFileStream = new FileStream($"{MessagesPath}/{messageId}", FileMode.Create))
			{
				Formatter.Serialize(newMessageFileStream, storedMessage);
			}
			return messageId;
		}

		void LetLastMessagePointToNewMessage(string lastMessageId, string newMessageId)
		{
			using (var lastMessageStream = new FileStream($"{MessagesPath}/{lastMessageId}", FileMode.Open))
			{
				var lastMessage = (StoredMessage)Formatter.Deserialize(lastMessageStream);
				lastMessage.Next = newMessageId;
				lastMessageStream.SetLength(0);
				lastMessageStream.Seek(0, SeekOrigin.Begin);
				Formatter.Serialize(lastMessageStream, lastMessage);
			}
		}
	}
}
