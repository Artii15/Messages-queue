using System;
using ServiceStack.DataAnnotations;

namespace Server.Entities
{
	[CompositeIndex(true, new[] {"SubscriberId", "TopicId"})]
	public class Subscription
	{
		[AutoIncrement]
		public int Id { get; set; }
		public int SubscriberId { get; set; }
		public int TopicId { get; set; }
		[Index]
		public DateTime CreationTime { get; set; }
		[References(typeof(Announcement))]
		public int? LastAnnouncementId { get; set; }
	}
}