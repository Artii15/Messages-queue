using ServiceStack.Common.Web;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Net;
using System.Data;

namespace Server
{
	[Authenticate]
	public class CreateSubscriptionService : Service
	{
		readonly CreatingSubscription CreatingSubscription;

		public CreateSubscriptionService()
		{
			CreatingSubscription = new CreatingSubscription(Db);
		}

		public object Post(CreateSubscription request)
		{
			try
			{
				CreatingSubscription.Create(request);
			}
			catch (TopicNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Queue {request.TopicName} not exists");
			}

			return new HttpResult(new CreateSubscriptionResponse(), HttpStatusCode.Created);

		}
	}
}
