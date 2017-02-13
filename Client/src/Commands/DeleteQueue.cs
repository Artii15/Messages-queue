using System;
using System.Net;
using RestSharp;

namespace Client.Commands
{
	public class DeleteQueue: Command
	{
		public DeleteQueue(RestClient client) : base(client) { }

		public override void Execute()
		{
			Console.Write("Queue name: ");
			switch (SendRequest(Reader.ReadNonEmptyString()))
			{
				case HttpStatusCode.NoContent:
					Console.WriteLine("Queue deleted");
					break;
				case HttpStatusCode.NotFound:
					Console.WriteLine("Queue with provided name not exists");
					break;
				default:
					Console.WriteLine("Something went wrong");
					break;
			}
		}

		HttpStatusCode SendRequest(string queueName)
		{
			var request = new RestRequest($"queues/{queueName}", Method.DELETE);
			return Client.Execute(request).StatusCode;
		}
	}
}
