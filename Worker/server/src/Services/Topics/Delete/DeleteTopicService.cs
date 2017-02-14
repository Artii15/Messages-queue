using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Topics.Delete
{
	public class DeleteTopicService: Service
	{
		readonly DeletingTopic DeletingTopic;

		public DeleteTopicService(DeletingTopic deletingTopic)
		{
			DeletingTopic = deletingTopic;
		}

		public DeleteTopicResponse Delete(DeleteTopic request)
		{
			DeletingTopic.Delete(request);
			return new DeleteTopicResponse();
		}
	}
}
