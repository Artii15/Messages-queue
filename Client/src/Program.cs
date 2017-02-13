using System;
using System.Collections.Generic;
using Client.Commands;
using RestSharp;

namespace Client
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Usage: client <service_address>");
				Environment.Exit((int)ProgramError.NoServiceAddress);
			}

			RunClient(args[0]);
		}

		static void RunClient(string serviceAddress)
		{
			try
			{
				Interact(new RestClient(serviceAddress));
			}
			catch (UriFormatException)
			{
				Console.WriteLine($"Invalid service address");
				Environment.Exit((int)ProgramError.InvalidAddress);
			}
		}

		static void Interact(RestClient client)
		{
			var commands = new Dictionary<string, Command>
			{
				{"1", new CreateQueue(client)},
				{"2", new DeleteQueue(client)},
				{"13", new EmptyCommand(client)}
			};
			var selectedCommand = "";
			while (selectedCommand != "13")
			{
				selectedCommand = SelectCommand(commands);
				commands[selectedCommand].Execute();
			}
		}

		static string SelectCommand(Dictionary<string, Command> commands)
		{
			Console.WriteLine("1 - Create queue");
			Console.WriteLine("2 - Delete queue");
			Console.WriteLine("3 - Read message");
			Console.WriteLine("4 - Create message");
			Console.WriteLine("5 - Create topic");
			Console.WriteLine("6 - Delete topic");
			Console.WriteLine("7 - Subscribe topic");
			Console.WriteLine("8 - Unsubscribe topic");
			Console.WriteLine("9 - Read announcement");
			Console.WriteLine("10 - Create announcement");
			Console.WriteLine("11 - List queues");
			Console.WriteLine("12 - List topics");
			Console.WriteLine("13 - Exit");

			var selectedCommand = "";
			while (!commands.ContainsKey(selectedCommand))
			{
				selectedCommand = Reader.ReadNonEmptyString();
			}
			return selectedCommand;
		}
	}
}