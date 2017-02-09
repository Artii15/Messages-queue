using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Messages.Get
{
	public class ReadMessageService: IService
	{
		readonly ReadingMessage ReadingMessage;

		public ReadMessageService(ReadingMessage readingMessage)
		{
			ReadingMessage = readingMessage;
		}

		public ReadMessageResponse Get(ReadMessage request)
		{
			var nextMessage = ReadingMessage.ReadNextFrom(request.QueueName);
			return new ReadMessageResponse { Content = nextMessage.Content, Id = nextMessage.Id };
		}
	}
}
