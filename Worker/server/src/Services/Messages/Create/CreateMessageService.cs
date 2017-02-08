using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Messages.Create
{
	public class CreateMessageService: IService
	{
		readonly CreatingMessage CreatingMessage;

		public CreateMessageService(CreatingMessage creatingMessage)
		{
			CreatingMessage = creatingMessage;
		}

		public CreateMessageResponse Post(CreateMessage request)
		{
			CreatingMessage.Create(request);
			return new CreateMessageResponse();
		}
	}
}
