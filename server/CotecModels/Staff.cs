using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class Staff
    {
        public Staff()
        {
            HospitalWorkers = new HashSet<HospitalWorkers>();
        }

        public string IdCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public virtual ICollection<HospitalWorkers> HospitalWorkers { get; set; }
    }
}
