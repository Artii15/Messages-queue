using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Subscriptions.Create
{
	public class CreateSubscriptionService: Service
	{
		readonly CreatingSubscription CreatingSubscription;

		public CreateSubscriptionService(CreatingSubscription creatingSubscription)
		{
			CreatingSubscription = creatingSubscription;
		}

		public CreateSubscriptionResponse Post(CreateSubscription request)
		{
			CreatingSubscription.Create(request);
			return new CreateSubscriptionResponse();
		}
	}
}
