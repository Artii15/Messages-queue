using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Timers;
using RestSharp;

namespace Server
{
    class Program
    {
		static void Main (string[] args)
        {
			if (args.Length != 1) 
			{
				Console.Error.WriteLine ("Usage: server <listen_address>");
				Environment.Exit(1);
			}
			var addressPattern = new Regex(@"^(http|https)://.+(:d+)?/?$");
			var listenAddress = string.Concat(args[0].TrimEnd(new char[] {'/'}), "/");
			if (!addressPattern.IsMatch(listenAddress))
			{
				Console.Error.WriteLine ("Invalid address format");
				Environment.Exit(2);
			}

			if (listenAddress.Contains("127.0.0.1")
			   || listenAddress.Contains("localhost")
			   || listenAddress.Contains("://*")
			   || listenAddress.Contains("0.0.0.0"))
			{
				Console.Error.WriteLine("Use specific address visible outside");
				Environment.Exit(3);
			}

			var appHost = new AppHost(listenAddress);
            appHost.Init ();            
			appHost.Start (listenAddress);
			appHost.BeginRecovery();
			BeginHeartbeat(listenAddress);

			Console.WriteLine ("AppHost Created at {0}, listening on {1}", DateTime.Now, listenAddress);
			Console.WriteLine("Press <ENTER> key to exit...");
			Console.ReadLine();
			appHost.Stop();
        }

		static void BeginHeartbeat(string ownAddress)
		{
			var heartbeatClient = new RestClient(ConfigurationManager.AppSettings["CoordinatorAddress"]);
			var heartbeatRequest = new RestRequest(ConfigurationManager.AppSettings["HeartbeatPath"], Method.POST);
			heartbeatRequest.RequestFormat = DataFormat.Json;
			heartbeatRequest.AddJsonBody(new { Address = ownAddress, Id = int.Parse(ConfigurationManager.AppSettings["Id"]) });

			var heartbeatInterval = double.Parse(ConfigurationManager.AppSettings["HeartbeatInterval"]);
			var timer = new Timer(heartbeatInterval);
			timer.AutoReset = true;
			timer.Elapsed += (sender, e) => SendHeartbeat(heartbeatClient, heartbeatRequest);
			SendHeartbeat(heartbeatClient, heartbeatRequest);
			timer.Start();
		}

		static void SendHeartbeat(RestClient client, RestRequest request)
		{
			client.Execute(request);
		}
    }
}