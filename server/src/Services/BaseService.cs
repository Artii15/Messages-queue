using System;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;

namespace Server.Services
{
    public class BaseService : Service
    {
        protected int GetCurrentAuthUserId()
        {
            var session = this.GetSession();
            if (session.IsAuthenticated == false)
                throw new HttpError(HttpStatusCode.Unauthorized, "No authorized sesssion");

            int id;
            if (!int.TryParse(session.UserAuthId, out id))
                throw new Exception("Unexpected UserAuthId, cannot parse as int");

            return id;
        }
    }
}
