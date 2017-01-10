using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Topics.Publish
{
	public class PublishService: Service
	{
		readonly Publishing Publishing;

		public PublishService(Publishing publishing)
		{
			Publishing = publishing;
		}

		public PublishResponse Post(Publish request)
		{
			Publishing.Publish(request.QueueName, request.TopicName, request.Message);
			return new PublishResponse();
		}
	}
}
