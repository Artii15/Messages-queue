using System;
using ServiceStack.DataAnnotations;

namespace Server.Entities
{
	public class Announcement
	{
		[AutoIncrement]
		public int Id { get; set; }
		public string Content { get; set; }
		[Index]
		public DateTime CreationTime { get; set; }
	}
}