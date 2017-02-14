using ServiceStack.Common.Web;
using ServiceStack.Common;
using ServiceStack.ServiceInterface;
using System.Net;

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
				var subscriberId = this.GetSession().UserAuthId.ToInt();
				CreatingSubscription.Create(request, subscriberId);
			}
			catch (TopicNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Topic {request.TopicName} not exists");
			}
			catch (BadRequestException)
			{
				return new HttpError(HttpStatusCode.BadRequest, "BadRequest");
			}

			return new HttpResult(new CreateSubscriptionResponse(), HttpStatusCode.Created);

		}
	}
}
