namespace Server.Storage
{
	public interface MessagesStorage
	{
		void Create(string queueName, string message);
		string TryToReadNextMessage(string queueName);
	}
}
