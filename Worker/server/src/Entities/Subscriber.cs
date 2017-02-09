using System;
using ServiceStack.DataAnnotations;

namespace Server.Entities
{
	public class Subscriber
	{
		[PrimaryKey]
		public int Id { get; set; }

		[Index]
		public DateTime CreationTime { get; set; }

		[References(typeof(Announcement))]
		public int? LastAnnouncementId { get; set; }
	}
}