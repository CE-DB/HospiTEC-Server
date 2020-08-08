using System;
using System.Collections.Generic;

namespace HospiTec_Server.database.DBModels
{
    public partial class Patient
    {
        public Patient()
        {
            ClinicRecord = new HashSet<ClinicRecord>();
            Reservation = new HashSet<Reservation>();
        }

        public string Identification { get; set; }
        public string PatientPassword { get; set; }

        public virtual Person IdentificationNavigation { get; set; }
        public virtual ICollection<ClinicRecord> ClinicRecord { get; set; }
        public virtual ICollection<Reservation> Reservation { get; set; }
    }
}
