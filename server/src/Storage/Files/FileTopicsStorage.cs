using System.IO;

namespace Server.Storage.Files
{
	public class FileTopicsStorage: TopicsStorage
	{
		readonly Paths Paths;

		public FileTopicsStorage(Paths paths)
		{
			Paths = paths;
		}

		public void Create(string queueName, string topicName)
		{
			Directory.CreateDirectory(Paths.GetTopicPath(queueName, topicName));
			Directory.CreateDirectory(Paths.GetTopicMessages(queueName, topicName));
			Directory.CreateDirectory(Paths.GetTopicPointers(queueName, topicName));
			File.Create(Paths.GetTopicPointer(queueName, topicName, PointersNames.First)).Close();
			File.Create(Paths.GetTopicPointer(queueName, topicName, PointersNames.Last)).Close();
		}
	}
}
