using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class ContentionMeasure
    {
        public ContentionMeasure()
        {
            ContentionMeasuresChanges = new HashSet<ContentionMeasuresChanges>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ContentionMeasuresChanges> ContentionMeasuresChanges { get; set; }
    }
}
