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

		public GetNextMessageResponse Get(GetNextMessage response)
		{
			var message = FetchingNextMessage.Fetch();
			return new GetNextMessageResponse { Message = message };
		}
	}
}