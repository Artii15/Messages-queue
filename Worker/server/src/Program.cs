using System;
using System.Timers;

namespace Server
{
    class Program
    {
		static void Main (string[] args)
        {
			if (args.Length > 1) 
			{
				Console.Error.WriteLine ("Usage: server <port_number>");
				Environment.Exit(1);
			}
			ushort port = 8888;
			if (args.Length == 1 && !ushort.TryParse(args[0], out port))
			{
				Console.Error.WriteLine ("Wrong port number");
				Environment.Exit(2);
			}

			var appHost = new AppHost();
            appHost.Init ();            
            var listeningOn = string.Format ("http://*:{0}/", port);
            appHost.Start (listeningOn);

			BeginHeartbeat();

            Console.WriteLine ("AppHost Created at {0}, listening on {1}", DateTime.Now, listeningOn);
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