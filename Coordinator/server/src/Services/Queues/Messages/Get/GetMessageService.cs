using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using System.Net;
using System;

namespace Server
{
	[Authenticate]
	public class GetMessageService : Service
	{
		readonly GettingMessage GettingMessage;

		public GetMessageService()
		{
			GettingMessage = new GettingMessage(Db);
		}

		public object Get(GetMessage request)
		{
			Message message;
			try
			{
				message = GettingMessage.Get(request);
			}
			catch (QueueNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Queue {request.QueueName} not exists");
			}

			return new GetMessageResponse()
			{
				Id = message.Id,
				Content = message.Content
			};
		}
	}
}
