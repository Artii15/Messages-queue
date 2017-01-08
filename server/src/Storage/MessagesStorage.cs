using Server.Entities;

namespace Server.Storage
{
	public interface MessagesStorage
	{
		void Create(string queueName, string message);
		Message? TryToReadNextMessage(string queueName);
	}
}
