using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class HospitalWorkers
    {
        public string IdCode { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }

        public virtual Hospital Hospital { get; set; }
        public virtual Staff IdCodeNavigation { get; set; }
    }
}
