﻿using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Net;
using System.Data;
namespace Server
{
	public class DeleteTopicService : Service
	{
		readonly DeletingTopic DeletingTopic;

		public DeleteTopicService()
		{
			DeletingTopic = new DeletingTopic(Db);
		}

		[RequiredPermission("create")]
		public object Delete(DeleteTopic request)
		{
			IDbTransaction transaction = Db.OpenTransaction();
			try
			{
				DeletingTopic.Delete(request);
			}
			catch (TopicNotExistsException)
			{
				return new HttpError(HttpStatusCode.NotFound, $"Topic {request.TopicName} not exists");
			}
			catch (BadRequestException)
			{
				return new HttpError(HttpStatusCode.BadRequest, "BadRequest");
			}
			finally
			{
				transaction.Commit();
			}

			return new HttpResult(new DeleteTopicResponse(), HttpStatusCode.NoContent);
		}
	}
}
