using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class SanitaryMeasure
    {
        public SanitaryMeasure()
        {
            SanitaryMeasuresChanges = new HashSet<SanitaryMeasuresChanges>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<SanitaryMeasuresChanges> SanitaryMeasuresChanges { get; set; }
    }
}
