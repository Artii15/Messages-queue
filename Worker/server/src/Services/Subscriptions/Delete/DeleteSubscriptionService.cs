using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Subscriptions.Delete
{
	public class DeleteSubscriptionService: IService
	{
		readonly DeletingSubscription DeletingSubscription;

		public DeleteSubscriptionService(DeletingSubscription deletingSubscription)
		{
			DeletingSubscription = deletingSubscription;
		}

		public DeleteSubscriptionResponse Delete(DeleteSubscription request)
		{
			DeletingSubscription.Delete(request);
			return new DeleteSubscriptionResponse();
		}
	}
}
