using System;
using System.Linq;
using Server.Services;
using Server.Services.PatientService;
using ServiceStack.ServiceClient.Web;

namespace client
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var client = new JsonServiceClient ("http://localhost:8888/")
			{
				UserName = "user",
				Password = "pass"
			};

			Console.WriteLine (client.Get (new Hello {Name = "World"}).Result);

			//DtoPatientResponse resp = client.Get (new DtoPatient ());

			//Console.WriteLine ("The result:");
			//foreach (var p in resp.Patients)
			//	Console.WriteLine (p.Id);
		}
	}
}
