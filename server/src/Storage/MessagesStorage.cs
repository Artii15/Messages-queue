using ServiceStack.FluentValidation.Resources;

namespace Server.Storage
{
	public interface MessagesStorage
	{
		void Create(string queueName, Messages message);
	}
}
