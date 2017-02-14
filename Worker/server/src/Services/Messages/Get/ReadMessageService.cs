using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Messages.Get
{
	public class ReadMessageService: Service
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
