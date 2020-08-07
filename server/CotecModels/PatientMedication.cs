using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class PatientMedication
    {
        public string PatientId { get; set; }
        public string Medication { get; set; }

        public virtual Medication MedicationNavigation { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
