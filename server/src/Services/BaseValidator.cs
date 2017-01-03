using System;
using System.Data;
using Server.Logic.Patients;
using ServiceStack.CacheAccess;
using ServiceStack.FluentValidation;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;


namespace Server.Services
{
    public abstract class BaseValidator<T> : AbstractValidator<T>
    {
        public IDbConnection Db { get; set; }
        public ICacheClient CacheClient { get; set; }

        protected bool PatientWithIdExists (int id)
        {
            //var result = Db.Select<Patient> (q => q.Id == id);

            //return result != null && result.Count > 0;
			return true;
        }


		protected bool PatientWithIdExistsForSessionUser (int id)
		{
			var sessionKey = SessionFeature.GetSessionKey ();


			var user = CacheClient.Get<AuthUserSession> (sessionKey);

			int uid;
			if (!int.TryParse (user.UserAuthId, out uid))
				throw new Exception ("Unexpected UserAuthId, cannot parse as int");

			var result = Db.Select<Patient> (q => q.OwnerId == uid && q.Id == id);

			return result != null && result.Count > 0;
		}
    }
}
