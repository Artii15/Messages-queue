using System;
using System.Configuration;
using System.Timers;
using RestSharp;

namespace Server
{
	public class HeartbeatSender
	{
		readonly TokenGenerator TokenGenerator = new TokenGenerator(Environment.GetEnvironmentVariable("APP_KEY"));
		readonly RestClient HeartbeatClient = new RestClient(Environment.GetEnvironmentVariable("COORDINATOR_ADDRESS"));
		readonly int WorkerId = int.Parse(Environment.GetEnvironmentVariable("WORKER_ID"));
		Timer Timer = new Timer(double.Parse(ConfigurationManager.AppSettings["HeartbeatInterval"]));
		readonly string HeartbeatPath = ConfigurationManager.AppSettings["HeartbeatPath"];
		readonly string ListenAddress;

		public HeartbeatSender(string listenAddress)
		{
			ListenAddress = listenAddress;
		}

		public void BeginHeartbeat()
		{
			Timer.AutoReset = true;
			Timer.Elapsed += (sender, e) => SendHeartbeat();
			SendHeartbeat();
			Timer.Start();
		}

		void SendHeartbeat()
		{
			var heartbeatRequest = new RestRequest(HeartbeatPath, Method.POST);
			heartbeatRequest.RequestFormat = DataFormat.Json;
			var tokenPair = TokenGenerator.Generate();

			heartbeatRequest.AddJsonBody(new
			{
				Address = ListenAddress,
				Id = WorkerId,
				tokenPair.Time,
				tokenPair.Token
			});

			HeartbeatClient.Execute(heartbeatRequest);
		}
	}
}
