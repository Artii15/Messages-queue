using System;

namespace Server.Services
{
	public static class Paths
	{
		public static string GetLastSegment(string path)
		{
			var segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			return segments[segments.Length - 1];
		}
	}
}
