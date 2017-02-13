using RestSharp;

namespace Client.Commands
{
	public abstract class Command
	{
		readonly protected RestClient Client;

		protected Command(RestClient client)
		{
			Client = client;
		}

		public abstract void Execute();
	}
}
