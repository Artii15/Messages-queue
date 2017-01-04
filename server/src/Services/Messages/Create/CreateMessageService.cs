using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Messages.Create
{
	public class CreateMessageService: Service
	{
		CreatingMessage CreatingMessage;

		public CreateMessageService(CreatingMessage creatingMessage)
		{
			CreatingMessage = creatingMessage;
		}

		public CreateMessageResponse Post(CreateMessage request)
		{
			CreatingMessage.Create(request.Message);
			return new CreateMessageResponse();
		}
	}
}