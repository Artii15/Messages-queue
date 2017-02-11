using System;
using System.Text.RegularExpressions;
using System.Timers;

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
			var addressPattern = new Regex(@"^(http|https)://.+(:d+)?$");
			string listenAddress = args[0];
			if (!addressPattern.IsMatch(listenAddress))
			{
				Console.Error.WriteLine ("Invalid address format");
				Environment.Exit(2);
			}

			var appHost = new AppHost(listenAddress);
            appHost.Init ();            
			appHost.Start (listenAddress);
			appHost.BeginRecovery();
			BeginHeartbeat();

			Console.WriteLine ("AppHost Created at {0}, listening on {1}", DateTime.Now, listenAddress);
			Console.WriteLine("Press <ENTER> key to exit...");
			Console.ReadLine();
			appHost.Stop();
        }

		static void BeginHeartbeat()
		{
			var heartbeatInterval = 10000; //TODO read interval from config or env variable
			var timer = new Timer(heartbeatInterval);
			timer.AutoReset = true;
			timer.Elapsed += (sender, e) => SendHeartbeat();
			SendHeartbeat();
			timer.Start();
		}

		static void SendHeartbeat()
		{
			//TODO Implement heartbeat sending here
			Console.WriteLine("heartbeat");
		}
    }
}