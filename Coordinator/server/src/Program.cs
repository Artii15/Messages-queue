﻿using System;
using System.Threading;

namespace Server
{
    class Program
    {
		static void Main (string[] args)
        {
			if (args.Length > 2) 
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

			new ManualResetEventSlim(false).Wait();

			appHost.Stop();
        }
    }
}