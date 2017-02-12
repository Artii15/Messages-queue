using ServiceStack.ServiceInterface;

namespace Server
{
	[Authenticate]
	public class GetTopicsService : Service
	{
		readonly GettingTopics GettingTopics;

		public GetTopicsService()
		{
			GettingTopics = new GettingTopics(Db);
		}

		public object Get(GetTopics request)
		{
			var topics = GettingTopics.Get();

			return new GetTopicsResponse()
			{
				Topics = topics
			};
		}
	}
}
