using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class Staff
    {
        public string Name { get; set; }
        public string Identification { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string StaffPassword { get; set; }

        public  Person IdentificationNavigation { get; set; }
        public  Role NameNavigation { get; set; }
    }
}
