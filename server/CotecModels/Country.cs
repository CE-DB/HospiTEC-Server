using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class Country
    {
        public Country()
        {
            ContentionMeasuresChanges = new HashSet<ContentionMeasuresChanges>();
            Population = new HashSet<Population>();
            Region = new HashSet<Region>();
            SanitaryMeasuresChanges = new HashSet<SanitaryMeasuresChanges>();
        }

        public string Name { get; set; }
        public string Continent { get; set; }

        public virtual Continent ContinentNavigation { get; set; }
        public virtual ICollection<ContentionMeasuresChanges> ContentionMeasuresChanges { get; set; }
        public virtual ICollection<Population> Population { get; set; }
        public virtual ICollection<Region> Region { get; set; }
        public virtual ICollection<SanitaryMeasuresChanges> SanitaryMeasuresChanges { get; set; }
    }
}
