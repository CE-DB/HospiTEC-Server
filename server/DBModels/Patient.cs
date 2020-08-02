using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class Patient
    {
        public Patient()
        {
            ClinicRecord = new HashSet<ClinicRecord>();
            Reservation = new HashSet<Reservation>();
        }

        public string Identification { get; set; }
        public string PatientPassword { get; set; }

        public  Person IdentificationNavigation { get; set; }
        public  ICollection<ClinicRecord> ClinicRecord { get; set; }
        public  ICollection<Reservation> Reservation { get; set; }
    }
}
