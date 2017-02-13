using System;
using System.Net;
using Client.Entities;
using RestSharp;

namespace Client.Commands
{
	public class ReadMessage: Command
	{
		public ReadMessage(RestClient client) : base(client) { }

		public override void Execute()
		{
			Console.Write("Queue name: ");
			try
			{
				Console.WriteLine(ReadQueue(Reader.ReadNonEmptyString()));
			}
			catch (ArgumentException)
			{
				Console.WriteLine("Invalid queue name");
			}
			catch (Exception)
			{
				Console.WriteLine("Something went wrong");
			}
		}

		ReadMessageResponse ReadQueue(string queueName)
		{
			ReadMessageResponse message = null;
			while (message == null)
			{
				var readResponse = SendReadRequest(queueName);
				message = TryConfirmRead(queueName, readResponse.Data) ? null : readResponse.Data;
			}
			return message;
		}

		IRestResponse<ReadMessageResponse> SendReadRequest(string queueName)
		{
			var request = new RestRequest($"queues/{queueName}/messages", Method.GET);
			var response = Client.Execute<ReadMessageResponse>(request);

			switch (response.StatusCode)
			{
				case HttpStatusCode.OK:
					return response;
				case HttpStatusCode.NotFound:
					throw new ArgumentException();
				default:
					throw new Exception();
			}
		}

		bool TryConfirmRead(string queueName, ReadMessageResponse readResponse)
		{
			var request = new RestRequest($"queues/{queueName}/messages/{readResponse.Id}", Method.DELETE);
			return Client.Execute(request).StatusCode == HttpStatusCode.NoContent;
		}
	}
}
