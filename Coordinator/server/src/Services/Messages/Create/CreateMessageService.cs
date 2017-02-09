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

		public object Post(CreateMessage request)
		{
			try
			{
				CreatingMessage.Create(request);
			}
			catch (QueueNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound,
				                     string.Format("Queue {0} not exists", request.QueueName));
			}

			return new HttpResult(new CreateQueueResponse(), HttpStatusCode.Created);
		}
	}
}
