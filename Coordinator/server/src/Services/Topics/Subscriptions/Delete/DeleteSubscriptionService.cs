using ServiceStack.Common.Web;
using ServiceStack.Common;
using ServiceStack.ServiceInterface;
using System.Net;

namespace Server
{
	[Authenticate]
	public class DeleteSubscriptionService : Service
	{
		readonly DeletingSubscription DeletingSubscription;

		public DeleteSubscriptionService()
		{
			DeletingSubscription = new DeletingSubscription(Db);
		}

		public object Delete(DeleteSubscription request)
		{
			try
			{
				var subscriberId = this.GetSession().UserAuthId.ToInt();
				DeletingSubscription.Delete(request, subscriberId);
			}
			catch (TopicNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Topic {request.TopicName} not exists");
			}
			catch (BadRequestException)
			{
				return new HttpError(HttpStatusCode.BadRequest, "BadRequest");
			}

			return new HttpResult(new CreateSubscriptionResponse(), HttpStatusCode.NoContent);

		}
	}
}
