using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using RestSharp;
using Server.Entities;
using Server.Services.Queues.Create;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;

namespace Server.Logic
{
	public class CreatingQueue
	{
		readonly ConcurrentDictionary<string, IDbConnectionFactory> Queues;

		public CreatingQueue(ConcurrentDictionary<string, IDbConnectionFactory> queues)
		{
			Queues = queues;
		}

		public void Create(CreateQueue request)
		{
			try
			{
				if (!string.IsNullOrEmpty(request.Cooperator))
				{
					PropagateRequestToCo(request);
				}
				CreateQueueFile(request.Name);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
		}

		void CreateQueueFile(string queueName)
		{
			var queueStoragePath = $"queues/{queueName}.sqlite";
			var queueDbConn = Queues.GetOrAdd(queueName,
							new OrmLiteConnectionFactory($"Data Source={queueStoragePath};Version=3;",
														 SqliteOrmLiteDialectProvider.Instance)).Open();
			queueDbConn.CreateTableIfNotExists<QueueMessage>();
			queueDbConn.Close();
		}

		void PropagateRequestToCo(CreateQueue request)
		{
			var client = new RestClient(request.Cooperator);

			var requestToSend = new RestRequest("queues", Method.POST);
			requestToSend.AddParameter("Name", request.Name);

			client.Execute(requestToSend);
		}

		IPAddress GetIpAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip;
				}
			}
			return null;
		}
	}
}