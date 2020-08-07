using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class PatientState
    {
        public PatientState()
        {
            Patient = new HashSet<Patient>();
        }

        public string Name { get; set; }

        public virtual ICollection<Patient> Patient { get; set; }
    }
}
