using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class Patient
    {
        public Patient()
        {
            PatientContact = new HashSet<PatientContact>();
            PatientMedication = new HashSet<PatientMedication>();
            PatientPathology = new HashSet<PatientPathology>();
        }

        public string Identification { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Region { get; set; }
        public string Nationality { get; set; }
        public string Country { get; set; }
        public byte Age { get; set; }
        public bool IntensiveCareUnite { get; set; }
        public bool Hospitalized { get; set; }
        public string State { get; set; }
        public DateTime DateEntrance { get; set; }

        public virtual Region RegionNavigation { get; set; }
        public virtual PatientState StateNavigation { get; set; }
        public virtual ICollection<PatientContact> PatientContact { get; set; }
        public virtual ICollection<PatientMedication> PatientMedication { get; set; }
        public virtual ICollection<PatientPathology> PatientPathology { get; set; }
    }
}
