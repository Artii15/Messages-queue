using System;
namespace Client
{
	public static class Reader
	{
		public static string ReadNonEmptyString()
		{
			var input = "";
			while (string.IsNullOrWhiteSpace(input))
			{
				input = Console.ReadLine();
			}
			return input;
		}
	}
}
