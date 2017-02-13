using System;
using RestSharp;

namespace Client.Commands
{
	public class CreateMessage : Command
	{
		public CreateMessage(RestClient client) : base(client) { }
		
		public override void Execute()
		{
			Console.Write("Queue name: ");
			var queueName = Reader.ReadNonEmptyString();
			Console.WriteLine("Message content: ");
			var messageContent = Console.ReadLine();

			switch (SendRequest(queueName, messageContent).StatusCode)
			{
				case System.Net.HttpStatusCode.NotFound:
					Console.WriteLine($"Queue {queueName} not exists");
					break;
				case System.Net.HttpStatusCode.Created:
					Console.WriteLine("Message sent");
					break;
				default:
					Console.WriteLine("Something went wrong");
					break;
			}
		}

		public IRestResponse SendRequest(string queueName, string messageContent)
		{
			var request = new RestRequest($"queues/{queueName}/messages", Method.POST);
			request.AddBody(new { Content = messageContent });
			return Client.Execute(request);
		}
	}
}
