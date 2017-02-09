using Server.Services.Subscriptions.Create;

namespace Server.Logic
{
	public class CreatingSubscription
	{
		Connections Connections;

		public CreatingSubscription(Connections connections)
		{
			Connections = connections;
		}

		public void Create(CreateSubscription request)
		{

		}
	}
}
