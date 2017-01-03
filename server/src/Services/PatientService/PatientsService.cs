using System;
using System.Linq;
using System.Net;
using Server.Logic.Patients;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceHost;

namespace Server.Services.PatientService
{
    [Authenticate]
    public class PatientsService : Service
    {
		private const string ALL_USERS_ID = "0";
        public PatientsService ()
        {
            Db.CreateTableIfNotExists<Patient>();
        }


		[RequiredPermission("read")]
        public object Get (DtoPatient req)
        {
			var cacheKey = req.Id == null ? ALL_USERS_ID : req.Id.ToString();

			return base.RequestContext.ToOptimizedResultUsingCache(this.Cache, cacheKey,()=>
				{
            		var patients = req.Id == null
                		? Db.Select<Patient> (q => q.OwnerId == GetCurrentAuthUserId ())
                		: Db.Select<Patient> (q => q.OwnerId == GetCurrentAuthUserId () && q.Id == req.Id);

            		var dtoPatients = patients.Select (p => new DtoPatient ().PopulateWith (p)).ToList ();

            		return new DtoPatientResponse (dtoPatients);
				});
        }


        public object Delete (DtoPatient req)
        {
            var found = Db.Select<Patient> (q => q.Id == req.Id && q.OwnerId == GetCurrentAuthUserId ());
            if (found.Count == 0)
                return new HttpResult {StatusCode = HttpStatusCode.NotFound};

            Db.DeleteById<Patient> (req.Id);

			base.RequestContext.RemoveFromCache (this.Cache, req.Id.ToString(), ALL_USERS_ID);

            return new HttpResult {StatusCode = HttpStatusCode.NoContent};
        }
         

		[RequiredRole ("editor")]
        public object Post (DtoPatient req)
        {
            var patient = Db.Select<Patient> (q => q.Id == req.Id && q.OwnerId == GetCurrentAuthUserId ())
                .SingleOrDefault ();
            if (patient != null)
                return new HttpResult { StatusCode = HttpStatusCode.Conflict };

            var newPatient = new Patient ().PopulateWith (req);
            newPatient.CreatedAt = DateTime.Now;
            newPatient.OwnerId = GetCurrentAuthUserId ();
            Db.Insert (newPatient);
            var id = (int) Db.GetLastInsertId ();

            req.PopulateWith (newPatient);
            req.Id = id;

			base.RequestContext.RemoveFromCache (this.Cache, ALL_USERS_ID);

            return new HttpResult(req)
            {
                StatusCode = HttpStatusCode.Created,
                Headers =
                               {
                                   {HttpHeaders.Location, base.Request.AbsoluteUri.CombineWith (id)}
                               }
            };
        }


        public object Put (DtoPatient req)
        {
            var patient = Db.Select<Patient>(q => q.Id == req.Id && q.OwnerId == GetCurrentAuthUserId())
                .SingleOrDefault();
            if (patient == null)
                return new HttpResult { StatusCode = HttpStatusCode.NotFound };

            patient.PopulateWith (req);
            Db.Update (patient);

			base.RequestContext.RemoveFromCache (this.Cache, req.Id.ToString(), ALL_USERS_ID);

            return new HttpResult
            {
                StatusCode = HttpStatusCode.NoContent,
                Headers =
                               {
                                   {HttpHeaders.Location, RequestContext.AbsoluteUri}
                               }
            };
        }
    }
}
