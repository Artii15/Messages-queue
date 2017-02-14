using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using System.Net;

namespace Server
{
	[Authenticate]
	public class CreateMessageService : Service
	{
		readonly CreatingMessage CreatingMessage;

		public CreateMessageService()
		{
			CreatingMessage = new CreatingMessage(Db);
		}

		[RequiredPermission("write")]
		public object Post(CreateMessage request)
		{
			try
			{
				CreatingMessage.Create(request);
			}
			catch (QueueNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Queue {request.QueueName} not exists");
			}

			return new HttpResult(new CreateQueueResponse(), HttpStatusCode.Created);
		}
	}
}
