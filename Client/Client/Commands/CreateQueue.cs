﻿using System;
using System.Net;
using RestSharp;

namespace Client.Commands
{
	public class CreateQueue: Command
	{
		public CreateQueue(RestClient client) : base(client) { }

		public void Create()
		{
			Console.Write("Queue name: ");
			switch (SendCreateRequest(Reader.ReadNonEmptyString()))
			{
				case HttpStatusCode.Created:
					Console.WriteLine("Queue created");
					break;
				case HttpStatusCode.Conflict:
					Console.WriteLine("Queue with provided name already exists");
					break;
				default:
					Console.WriteLine("Something went wrong");
					break;
			}
		}

		HttpStatusCode SendCreateRequest(string queueName)
		{
			var request = new RestRequest("queues", Method.POST);
			request.AddBody(new { Name = queueName });
			return Client.Execute(request).StatusCode;
		}
	}
}
