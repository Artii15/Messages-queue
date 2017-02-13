using System;
using System.Net;
using RestSharp;

namespace Client.Commands
{
	public class CreateTopic : Command
	{
		public CreateTopic(RestClient client) : base(client) { }

		public override void Execute()
		{
			Console.Write("Topic name: ");
			switch (SendRequest(Reader.ReadNonEmptyString()))
			{
				case HttpStatusCode.Created:
					Console.WriteLine("Topic created");
					break;
				case HttpStatusCode.Conflict:
					Console.WriteLine("Topic with provided name already exists");
					break;
				default:
					Console.WriteLine("Something went wrong");
					break;
			}
		}

		HttpStatusCode SendRequest(string topicName)
		{
			var request = new RestRequest("topics", Method.POST);
			request.AddBody(new { Name = topicName });
			return Client.Execute(request).StatusCode;
		}
	}
}
