using System.IO;
using Server.Logic;
using ServiceStack.Common.Utils;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;

namespace Server.Services.Databases.Get
{
	public class GetDatabaseService: IService
	{
		readonly GettingDatabase GettingDatabase;

		public GetDatabaseService(GettingDatabase gettingDatabase)
		{
			GettingDatabase = gettingDatabase;
		}

		public HttpResult Get(GetDatabase request)
		{
			return new HttpResult(new FileInfo(GettingDatabase.Get(request).MapHostAbsolutePath()), asAttachment: false);
		}
	}
}