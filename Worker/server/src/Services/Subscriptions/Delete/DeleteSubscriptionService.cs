using Server.Logic;

namespace Server.Services.Subscriptions.Delete
{
	public class DeleteSubscriptionService
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
