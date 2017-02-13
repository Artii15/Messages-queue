using System;
using System.Net;
using RestSharp;

namespace Client.Commands
{
	public class ReadMessage: Command
	{
		public ReadMessage(RestClient client) : base(client) { }

		public override void Execute()
		{
			Console.Write("Queue name: ");

		}

		HttpStatusCode SendRequest(string queueName)
		{
			var request = new RestRequest($"queues/{queueName}/messages", Method.GET);
			return Client.Execute(request).StatusCode;
		}
	}
}
