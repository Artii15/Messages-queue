using System;
using ServiceStack.DataAnnotations;

namespace Server.Entities
{
	[CompositeIndex(true, new[] {"SubscriberId", "TopicId"})]
	public class Subscription
	{
		[AutoIncrement]
		public int Id;
		public int SubscriberId;
		public int TopicId;
		public DateTime CreationTime;
		[References(typeof(Announcement))]
		public int? LastAnnouncementId;
	}
}
