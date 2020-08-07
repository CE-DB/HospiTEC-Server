using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class PathologySymptoms
    {
        public string Pathology { get; set; }
        public string Symptom { get; set; }

        public virtual Pathology PathologyNavigation { get; set; }
    }
}
