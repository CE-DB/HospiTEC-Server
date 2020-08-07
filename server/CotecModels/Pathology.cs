using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class Pathology
    {
        public Pathology()
        {
            ContactPathology = new HashSet<ContactPathology>();
            PathologySymptoms = new HashSet<PathologySymptoms>();
            PatientPathology = new HashSet<PatientPathology>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Treatment { get; set; }

        public virtual ICollection<ContactPathology> ContactPathology { get; set; }
        public virtual ICollection<PathologySymptoms> PathologySymptoms { get; set; }
        public virtual ICollection<PatientPathology> PatientPathology { get; set; }
    }
}
