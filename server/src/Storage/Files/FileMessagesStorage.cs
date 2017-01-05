using System;
using System.IO;
using ServiceStack.FluentValidation.Resources;

namespace Server.Storage.Files
{
	public class FileMessagesStorage : MessagesStorage
	{
		Paths Paths;

		public FileMessagesStorage(Paths paths)
		{
			Paths = paths;
		}

		public void Create(string queueName, Messages message)
		{
			var messagesPath = Paths.MakeMessagesPath(queueName);
			EnsureMessagesDirExists(messagesPath);

			var messageId = Guid.NewGuid().ToString();

		}

		void EnsureMessagesDirExists(string messagesPath)
		{
			Directory.CreateDirectory(messagesPath);
		}
	}
}