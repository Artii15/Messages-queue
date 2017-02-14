using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Subscriptions.Delete
{
	public class DeleteSubscriptionService: Service
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
