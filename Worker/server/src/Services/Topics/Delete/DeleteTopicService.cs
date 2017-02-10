using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Topics.Delete
{
	public class DeleteTopicService: IService
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
