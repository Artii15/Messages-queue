using System.Collections.Generic;

namespace Server.Services.PatientService
{
    public class DtoPatientResponse
    {
        public DtoPatientResponse (IList<DtoPatient> patients)
        {
            Patients = patients;
        }

        public IList<DtoPatient> Patients { get; set; }



    }
}