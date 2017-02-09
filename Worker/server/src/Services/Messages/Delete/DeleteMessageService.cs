using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Messages.Delete
{
	public class DeleteMessageService: IService
	{
		readonly DeletingMessage DeletingMessage;

		public DeleteMessageService(DeletingMessage deletingMessage)
		{
			DeletingMessage = deletingMessage;
		}

		public DeleteMessageResponse Delete(DeleteMessage request)
		{
			DeletingMessage.Delete(request.QueueName, request.MessageId);
			return new DeleteMessageResponse();
		}
	}
}
