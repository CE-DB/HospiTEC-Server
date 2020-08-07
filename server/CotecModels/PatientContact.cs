using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class PatientContact
    {
        public string PatientId { get; set; }
        public string ContactId { get; set; }
        public DateTime LastVisit { get; set; }

        public virtual Contact Contact { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
