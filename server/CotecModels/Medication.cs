using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class Medication
    {
        public Medication()
        {
            PatientMedication = new HashSet<PatientMedication>();
        }

        public string Medicine { get; set; }
        public string Pharmacist { get; set; }

        public virtual ICollection<PatientMedication> PatientMedication { get; set; }
    }
}
