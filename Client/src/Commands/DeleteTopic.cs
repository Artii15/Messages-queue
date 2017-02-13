using System;
using System.Net;
using RestSharp;

namespace Client.Commands
{
	public class DeleteTopic : Command
	{
		public DeleteTopic(RestClient client) : base(client) { }

		public override void Execute()
		{
			Console.Write("Topic name: ");
			switch (SendRequest(Reader.ReadNonEmptyString()))
			{
				case HttpStatusCode.NoContent:
					Console.WriteLine("Topic deleted");
					break;
				case HttpStatusCode.NotFound:
					Console.WriteLine("Topic with provided name not exists");
					break;
				default:
					Console.WriteLine("Something went wrong");
					break;
			}
		}

		HttpStatusCode SendRequest(string topicName)
		{
			var request = new RestRequest($"topics/{topicName}", Method.DELETE);
			return Client.Execute(request).StatusCode;
		}
	}
}