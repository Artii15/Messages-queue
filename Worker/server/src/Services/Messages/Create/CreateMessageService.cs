using ServiceStack.ServiceHost;

namespace Server.Services.Messages.Create
{
	public class CreateMessageService: IService
	{
		public CreateMessageResponse Post(CreateMessage request)
		{
			return new CreateMessageResponse();
		}
	}
}
