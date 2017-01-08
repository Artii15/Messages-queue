using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Messages.Next
{
	public class GetNextMessageService: Service
	{
		FetchingNextMessage FetchingNextMessage;

		public GetNextMessageService(FetchingNextMessage fetchingNextMessage)
		{
			FetchingNextMessage = fetchingNextMessage;
		}

		public GetNextMessageResponse Get(GetNextMessage request)
		{
			var message = FetchingNextMessage.Fetch(request.QueueName);
			return new GetNextMessageResponse { Id = message.Id, Content = message.Content };
		}
	}
}