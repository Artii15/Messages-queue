using Server.Logic;

namespace Server.Services.Messages.Next
{
	public class GetNextMessageService
	{
		FetchingNextMessage FetchingNextMessage;

		public GetNextMessageService(FetchingNextMessage fetchingNextMessage)
		{
			FetchingNextMessage = fetchingNextMessage;
		}

		public GetNextMessageResponse Get(GetNextMessage request)
		{
			var message = FetchingNextMessage.Fetch(request.QueueName);
			return new GetNextMessageResponse { Message = message };
		}
	}
}