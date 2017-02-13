using RestSharp;

namespace Client.Commands
{
	public class EmptyCommand : Command
	{
		public EmptyCommand(RestClient client) : base(client) { }

		public override void Execute()
		{
		}
	}
}
