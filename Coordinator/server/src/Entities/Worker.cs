using System;

namespace Server
{
	public class Worker
	{
		public long Id { get; set; }
		public string Address { get; set; }
		public bool Alive { get; set; }
		public DateTime LastHeartbeat { get; set; }
	}
}
