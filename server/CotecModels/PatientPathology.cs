using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class PatientPathology
    {
        public string PathologyName { get; set; }
        public string PatientId { get; set; }

        public virtual Pathology PathologyNameNavigation { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
