using System;
using ServiceStack.ServiceHost;

namespace Server.Services.Announcements.Create
{
	[Route("/topics/{TopicName}/announcements", "post")]
	public class CreateAnnouncement
	{
		public string TopicName { get; set; }
		public string Content { get; set; }
		public string Cooperator { get; set; }
		public DateTime CreationTime { get; set; }
	}
}
