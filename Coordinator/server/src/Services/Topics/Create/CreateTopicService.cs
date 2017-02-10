using ServiceStack.Common.Web;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Net;
using System.Data;

namespace Server
{
	[Authenticate]
	public class CreateTopicService : Service
	{
		readonly CreatingTopic CreatingTopic;

		public CreateTopicService()
		{
			CreatingTopic = new CreatingTopic(Db);
			Db.CreateTableIfNotExists<Topic>();
		}

		public object Post(CreateTopic request)
		{
			IDbTransaction transaction = Db.OpenTransaction();
			try
			{
				CreatingTopic.Create(request);
			}
			catch (TopicAlreadyExistsException)
			{
				return new HttpError(HttpStatusCode.Conflict, $"Topic {request.Name} already exists");
			}
			finally
			{
				transaction.Commit();
			}

			return new HttpResult(new CreateQueueResponse())
			{
				StatusCode = HttpStatusCode.Created,
				Headers =
							   {
					{HttpHeaders.Location, base.Request.AbsoluteUri.CombineWith(request.Name)}
							   }
			};
		}
	}
}
