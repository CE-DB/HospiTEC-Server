using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public partial class Staff
    {
        public string Name { get; set; }
        public string Identification { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string StaffPassword { get; set; }

        public virtual Role NameNavigation { get; set; }
    }
}
