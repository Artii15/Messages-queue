using System;

namespace Client.Commands
{
	public class CreateQueue
	{
		public void Create()
		{
			Console.Write("Queue name: ");
			var queueName = Console.ReadLine();
		}
	}
}
