using System;
using System.Collections.Generic;

namespace HospiTec_Server.database.DBModels
{
    /// <summary>
    /// This class maps the entity staff
    /// </summary>
    public partial class Staff
    {
        public string Name { get; set; }
        public string Identification { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string StaffPassword { get; set; }

        public virtual Person IdentificationNavigation { get; set; }
        public virtual Role NameNavigation { get; set; }
    }
}
