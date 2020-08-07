using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class Hospital
    {
        public Hospital()
        {
            HospitalWorkers = new HashSet<HospitalWorkers>();
        }

        public string Name { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public short Capacity { get; set; }
        public short IcuCapacity { get; set; }
        public string Manager { get; set; }

        public virtual Contact ManagerNavigation { get; set; }
        public virtual Region RegionNavigation { get; set; }
        public virtual ICollection<HospitalWorkers> HospitalWorkers { get; set; }
    }
}
